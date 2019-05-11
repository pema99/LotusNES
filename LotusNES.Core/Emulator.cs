using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Linq;

namespace LotusNES.Core
{
    public static class Emulator
    {
        //Emulation components
        public static CPU CPU { get; private set; }
        public static PPU PPU { get; private set; }
        public static APU APU { get; private set; }
        public static GamePak GamePak { get; private set; }
        public static Mapper Mapper { get; private set; }
        public static Controller[] Controllers { get; private set; }

        public static NetPlayServer NetPlayServer { get; private set; }
        public static GameGenie GameGenie { get; private set; }
        public static RewindManager RewindManager { get; private set; }

        private static Thread EmuThread;

        //Emulation options
        public static bool Turbo { get; set; }
        public static double Speed { get; set; }
        public static bool Pause { get; set; }
        public static bool DisableAPU { get; set; }
        public static bool TrackHistory { get; set; }

        //State
        public static EmulatorState State { get; private set; }
        public static bool Running { get { return State != EmulatorState.Halted; } }
        public static bool ShouldUpdate { get { return State != EmulatorState.CatchingUp && Running; } }
        private static bool queueLoad;
        private static string romPath;
        private static bool queueSaveState;
        private static bool queueLoadState;
        private static string saveStatePath;

        //Misc
        public static Action<Exception> ErrorHandler { get; set; }

        public static void Initialize()
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
            CPU = new CPU();
            PPU = new PPU();
            APU = new APU();
            Controllers = new Controller[] { new Controller(), new Controller() };

            NetPlayServer = new NetPlayServer();
            GameGenie = new GameGenie();
            RewindManager = new RewindManager();

            //Init emulation thread
            EmuThread = new Thread(EmulationLoop);
            EmuThread.IsBackground = true;
            EmuThread.Start();
        }

        public static void LoadROM(string path)
        {
            queueLoad = true;
            romPath = path;
        }

        public static void SaveStateToFile(string path)
        {
            queueSaveState = true;
            saveStatePath = path;
        }

        public static void LoadStateFromFile(string path)
        {
            queueLoadState = true;
            saveStatePath = path;
        }

        internal static void WriteStateToStream(Stream stream)
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

        internal static void ReadStateFromStream(Stream stream)
        {
            BinaryFormatter BF = new BinaryFormatter();
            object[] data = (object[])BF.Deserialize(stream);
            CPU = (CPU)data[0];
            PPU = (PPU)data[1];
            APU = (APU)data[2];
            Mapper = (Mapper)data[3];
            if (GamePak.UsesCharRAM)
            {
                GamePak.LoadCharRAM((byte[])data[4]);
            }
        }

        public static void StartRewinding()
        {
            //Don't interrupt existing catchup
            if (ShouldUpdate)
            {
                //Block input while rewinding up
                Array.ForEach(Controllers, x => x.BlockInput = true);

                State = EmulatorState.Rewinding;
            }
        }

        public static void StopRewinding()
        {
            if (Running)
            {
                //Stop rewinding, catch up
                State = EmulatorState.CatchingUp;
            }
        }

        //Main loop
        private static void EmulationLoop()
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

        private static void HandleFrame()
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

        private static void DoFrame()
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
        private static void HandleIO()
        {
            if (queueLoadState)
            {
                queueLoadState = false;
                using (FileStream FS = new FileStream(saveStatePath, FileMode.Open))
                {
                    ReadStateFromStream(FS);
                }
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
                Mapper = Mapper.Create(GamePak);
                CPU.Reset();
                PPU.Reset();
                APU.Reset();

                State = EmulatorState.Running;
            }
        }
    }
}
