﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Linq;

namespace LotusNES.Core
{
    public class Emulator
    {
        //Emulation components
        public CPU CPU { get; private set; }
        public PPU PPU { get; private set; }
        public APU APU { get; private set; }
        public GamePak GamePak { get; private set; }
        public Mapper Mapper { get; private set; }
        public Controller[] Controllers { get; private set; }

        public NetPlayServer NetPlayServer { get; private set; }
        public GameGenie GameGenie { get; private set; }
        public RewindManager RewindManager { get; private set; }

        private Thread EmuThread;

        //Emulation options
        public bool Turbo { get; set; }
        public double Speed { get; set; }
        public bool Pause { get; set; }
        public bool DisableAPU { get; set; }
        public bool TrackHistory { get; set; }

        //State
        public EmulatorState State { get; private set; }
        public bool Running { get { return State != EmulatorState.Halted; } }
        public bool ShouldUpdate { get { return State != EmulatorState.CatchingUp && Running; } }
        private bool queueLoad;
        private string romPath;
        private bool queueSaveState;
        private bool queueLoadState;
        private string saveStatePath;

        //Misc
        public Action<Exception> ErrorHandler { get; set; }

        public Emulator()
        {
            //Set emulation settings
            Turbo = false;
            Speed = 1;
            Pause = false;
            DisableAPU = false;
            TrackHistory = false;

            State = EmulatorState.Halted;
            queueLoad = false;
            romPath = "";
            queueSaveState = false;
            queueLoadState = false;
            saveStatePath = "";

            //Init emulator
            CPU = new CPU(this);
            PPU = new PPU(this);
            APU = new APU(this);
            Controllers = new Controller[] { new Controller(), new Controller() };

            NetPlayServer = new NetPlayServer(this);
            GameGenie = new GameGenie();
            RewindManager = new RewindManager(this);

            //Init emulation thread
            EmuThread = new Thread(EmulationLoop);
            EmuThread.IsBackground = true;
            EmuThread.Start();
        }

        public void LoadROM(string path)
        {
            queueLoad = true;
            romPath = path;
        }

        public void SaveStateToFile(string path)
        {
            queueSaveState = true;
            saveStatePath = path;
        }

        public void LoadStateFromFile(string path)
        {
            queueLoadState = true;
            saveStatePath = path;
        }

        internal void WriteStateToStream(Stream stream)
        {
            BinaryFormatter BF = new BinaryFormatter();
            object[] data;
            if (GamePak.UsesCharRAM)
            {
                data = new object[] { CPU, PPU, APU, Mapper, GamePak.GetCharRAM() };
            }
            else
            {
                data = new object[] { CPU, PPU, APU, Mapper };
            }
            BF.Serialize(stream, data);
        }

        internal void ReadStateFromStream(Stream stream)
        {
            BinaryFormatter BF = new BinaryFormatter();
            object[] data = (object[])BF.Deserialize(stream);
            //Make sure to fix dead references from serialization
            CPU = (CPU)data[0];
            CPU.RefreshEmulatorReference(this);
            CPU.Memory.RefreshEmulatorReference(this);
            PPU = (PPU)data[1];
            PPU.RefreshEmulatorReference(this);
            PPU.Memory.RefreshEmulatorReference(this);
            APU = (APU)data[2];
            APU.RefreshEmulatorReference(this);
            Mapper = (Mapper)data[3];
            Mapper.RefreshEmulatorReference(this);
            if (GamePak.UsesCharRAM)
            {
                GamePak.LoadCharRAM((byte[])data[4]);
            }
        }

        public void StartRewinding()
        {
            //Don't interrupt existing catchup
            if (ShouldUpdate)
            {
                //Block input while rewinding up
                Array.ForEach(Controllers, x => x.BlockInput = true);

                State = EmulatorState.Rewinding;
            }
        }

        public void StopRewinding()
        {
            if (Running)
            {
                //Stop rewinding, catch up
                State = EmulatorState.CatchingUp;
            }
        }

        //Main loop
        private void EmulationLoop()
        {          
            while (true)
            {
                try
                {
                    HandleIO();
                    HandleFrame();
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    //Stop emulation, show error
                    State = EmulatorState.Halted;
                    ErrorHandler?.Invoke(e);
                }
            }
        }

        private void HandleFrame()
        {
            if (!Pause && Running)
            {
                Stopwatch SW = Stopwatch.StartNew();
                if (State == EmulatorState.Rewinding)
                {
                    var frame = RewindManager.PopFrame();
                    PPU.FrameBuffer = frame.frame;

                    if (frame.startReached)
                    {
                        Array.ForEach(Controllers, x => x.BlockInput = false);
                        State = EmulatorState.Running;
                    }
                }
                else if (State == EmulatorState.CatchingUp)
                {
                    //Catchup to current frame num
                    int numFrames = RewindManager.FrameOffset;
                    for (int n = 0; n < numFrames; n++)
                    {
                        DoFrame();

                        var frame = RewindManager.GetFrameRelative(n);
                        for (int i = 0; i < 2; i++)
                        {
                            Controllers[0].LoadInputByte(frame.input[0]);
                            Controllers[1].LoadInputByte(frame.input[1]);
                        }
                    }

                    Array.ForEach(Controllers, x => x.BlockInput = false);
                    State = EmulatorState.Running;
                }
                else
                {
                    DoFrame();

                    if (TrackHistory)
                    {
                        RewindManager.PushFrame(new byte[] { Controllers[0].GetInputByte(), Controllers[1].GetInputByte() }, (byte[])PPU.FrameBuffer.Clone());
                    }
                }
                SW.Stop();

                if (!Turbo)
                {
                    Thread.Sleep(Math.Max((int)(((1000.0 / 60) - SW.ElapsedMilliseconds) / Speed), 0));
                }
            }
        }

        private void DoFrame()
        {
            bool PreFrame = PPU.OddFrame;
            while (PreFrame == PPU.OddFrame)
            {
                ulong ElapsedCycles = CPU.Step();

                for (uint i = 0; i < ElapsedCycles && !DisableAPU; i++)
                {
                    APU.Step();
                }

                //3 PPU per CPU
                for (uint i = 0; i < ElapsedCycles * 3; i++)
                {
                    PPU.Step();

                    //Inform mapper
                    Mapper.Step();
                }
            }
        }

        //Save states and rom loading
        private void HandleIO()
        {
            if (queueLoadState)
            {
                queueLoadState = false;
                using (FileStream FS = new FileStream(saveStatePath, FileMode.Open))
                {
                    ReadStateFromStream(FS);
                }
                RewindManager.Reset();
            }

            else if (queueSaveState)
            {
                queueSaveState = false;
                using (FileStream FS = new FileStream(saveStatePath, FileMode.OpenOrCreate))
                {
                    WriteStateToStream(FS);
                }
            }

            else if (queueLoad)
            {
                queueLoad = false;

                GamePak = new GamePak(romPath);
                Mapper = Mapper.Create(this);
                CPU.Reset();
                PPU.Reset();
                APU.Reset();
                RewindManager.Reset();

                State = EmulatorState.Running;
            }
        }
    }
}
