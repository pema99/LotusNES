using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Lotus6502
{
    public static class Emulator
    {
        //UI
        public static MainForm Form;
        public static MainViewport Viewport;

        //Emulation components
        public static CPU CPU;
        public static PPU PPU;
        public static APU APU;
        public static GamePak GamePak;
        public static Mapper Mapper;
        public static Controller Controller1;
        public static Controller Controller2;
        public static NetPlayServer NetPlayServer;

        private static Thread EmuThread;

        //Emulation options
        public static bool Turbo;
        public static double Speed;
        public static bool Pause;

        //State
        public static bool Running;
        private static bool queueLoad;
        private static string romPath;
        private static bool queueSaveState;
        private static bool queueLoadState;
        private static string saveStatePath;

        public static void Initialize()
        {
            //Set emulation settings
            Turbo = false;
            Speed = 1;
            Pause = false;

            Running = false;
            queueLoad = false;
            romPath = "";
            queueSaveState = false;
            queueLoadState = false;
            saveStatePath = "";

            //Init emulator
            CPU = new CPU();
            PPU = new PPU();
            APU = new APU();
            Controller1 = new Controller();
            Controller2 = new Controller();

            NetPlayServer = new NetPlayServer();

            //Init ui and emulation thread
            Application.EnableVisualStyles();
            Form = new MainForm();
            Viewport = new MainViewport();

            EmuThread = new Thread(EmulationLoop);
            EmuThread.IsBackground = true;
            EmuThread.Start();

            Form.Show();
            Viewport.Run();
        }

        public static void SaveState(string path)
        {
            queueSaveState = true;
            saveStatePath = path;
        }

        public static void LoadState(string path)
        {
            queueLoadState = true;
            saveStatePath = path;
        }

        public static void LoadROM(string path)
        {
            queueLoad = true;
            romPath = path;
        }

        //Main loop
        private static void EmulationLoop()
        {          
            while (true)
            {
                try
                {
                    HandleIO();
                    DoFrame();
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    //Stop emulation, show error
                    Running = false; 
                    MessageBox.Show(
                        string.Format("Emulation error occurred: {0}", e.Message), 
                        "Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }
            }
        }

        private static void DoFrame()
        {
            if (!Pause && Running)
            {
                Stopwatch SW = Stopwatch.StartNew();
                bool PreFrame = PPU.OddFrame;
                while (PreFrame == PPU.OddFrame)
                {
                    int ElapsedCycles = CPU.Step();

                    for (int i = 0; i < ElapsedCycles; i++)
                    {
                        APU.Step();
                    }

                    //3 PPU per CPU
                    for (int i = 0; i < ElapsedCycles * 3; i++)
                    {
                        PPU.Step();

                        //Inform mapper
                        Mapper.Step();
                    }
                }
                SW.Stop();

                if (!Turbo)
                {
                    Thread.Sleep(Math.Max((int)(((1000.0 / 60) - SW.ElapsedMilliseconds) / Speed), 0));
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
                    BinaryFormatter BF = new BinaryFormatter();
                    object[] data = (object[])BF.Deserialize(FS);
                    CPU = (CPU)data[0];
                    PPU = (PPU)data[1];
                    APU = (APU)data[2];
                    Mapper = (Mapper)data[3];
                    if (GamePak.UsesCharRAM)
                    {
                        GamePak.LoadCharRAM((byte[])data[4]);
                    }
                }
            }

            else if (queueSaveState)
            {
                queueSaveState = false;
                using (FileStream FS = new FileStream(saveStatePath, FileMode.OpenOrCreate))
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
                    BF.Serialize(FS, data);
                }
            }

            else if (queueLoad)
            {
                GamePak = new GamePak(romPath);
                Mapper = Mapper.Create(GamePak);
                CPU.Reset();
                PPU.Reset();
                APU.Reset();

                Running = true;
                queueLoad = false;
            }
        }
    }
}
