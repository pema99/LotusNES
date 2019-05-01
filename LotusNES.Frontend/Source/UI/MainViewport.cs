using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public class MainViewport : Game
    {
        //Monogame stuff
        private GraphicsDeviceManager graphics;
        private SpriteBatch sb;
        private Texture2D blank;

        //Configuration
        private int screenWidth = 256;
        private int screenHeight = 240;
        private int screenScale = 1;

        //State
        private Color[] frameBuffer;
        private Texture2D frameBufferTexture;
        public bool ExitRequest { get; set; }

        //Sound
        private DynamicSoundEffectInstance sound;
        private byte[] audioBuffer;
        
        public MainViewport()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth * screenScale;
            graphics.PreferredBackBufferHeight = screenHeight * screenScale;
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            Window.AllowUserResizing = false;
        }

        protected override void Initialize()
        {
            sb = new SpriteBatch(GraphicsDevice);
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new Color[] { Color.White });
            frameBuffer = new Color[screenWidth * screenHeight];
            frameBufferTexture = new Texture2D(GraphicsDevice, screenWidth, screenHeight);

            sound = new DynamicSoundEffectInstance(44100, AudioChannels.Mono);
            audioBuffer = new byte[APU.BufferSize * 2];
            sound.Play();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (ExitRequest)
            {
                Exit();
            }

            if (Emulator.Running)
            {
                //Handle audio, don't store too many buffers or we will lag
                if (sound.PendingBufferCount < 5)
                {
                    HandleAudio();
                }

                //Input
                KeyboardState KB = Keyboard.GetState();
                GamePadState GP = GamePad.GetState(PlayerIndex.One);
                bool[] inputs = new bool[] 
                {
                    KB.IsKeyDown(Keys.Z) || GP.IsButtonDown(Buttons.A) || GP.IsButtonDown(Buttons.LeftShoulder)  || GP.IsButtonDown(Buttons.LeftTrigger),
                    KB.IsKeyDown(Keys.X) || GP.IsButtonDown(Buttons.B) || GP.IsButtonDown(Buttons.RightShoulder) || GP.IsButtonDown(Buttons.RightTrigger),

                    KB.IsKeyDown(Keys.A) || GP.IsButtonDown(Buttons.Back),
                    KB.IsKeyDown(Keys.S) || GP.IsButtonDown(Buttons.Start),

                    KB.IsKeyDown(Keys.Up)    || GP.IsButtonDown(Buttons.LeftThumbstickUp)    || GP.IsButtonDown(Buttons.DPadUp),
                    KB.IsKeyDown(Keys.Down)  || GP.IsButtonDown(Buttons.LeftThumbstickDown)  || GP.IsButtonDown(Buttons.DPadDown),
                    KB.IsKeyDown(Keys.Left)  || GP.IsButtonDown(Buttons.LeftThumbstickLeft)  || GP.IsButtonDown(Buttons.DPadLeft),
                    KB.IsKeyDown(Keys.Right) || GP.IsButtonDown(Buttons.LeftThumbstickRight) || GP.IsButtonDown(Buttons.DPadRight),
                };
                Emulator.Controller1.SetButtons(inputs);

                //Draw frame
                for (int i = 0; i < screenWidth * screenHeight; i++)
                {
                    frameBuffer[i] = PaletteMap.MapXNA(Emulator.PPU.FrameBuffer[i]);
                }

                //Get input from peer
                if (Emulator.NetPlayServer.Running)
                {                                
                    while (Emulator.NetPlayServer.GetPeerInputAvailable())
                    {
                        Emulator.Controller2.SetButtons(Emulator.NetPlayServer.GetPeerInput());
                    }
                }

                frameBufferTexture.SetData(frameBuffer);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sb.Begin(samplerState: SamplerState.PointClamp);

            if (Emulator.GamePak != null)
            {
                //FrameBuffer
                sb.Draw(frameBufferTexture, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            sb.End();

            //Send new frame to peer
            if (Emulator.NetPlayServer.Running)
            {
                Emulator.NetPlayServer.SendPeerFrameBuffer(); //TODO: ONLY SEND NEW PPU FRAMES
            }

            base.Draw(gameTime);
        }

        private void HandleAudio()
        {
            //If enough has been gathered to submit, do so
            if (Emulator.APU.GetAudioBufferReady())
            {
                float[] buffer = Emulator.APU.GetAudioBuffer();

                for (int i = 0; i < APU.BufferSize; i++)
                {
                    float sample = MathHelper.Clamp(buffer[i], 0, 1);
                    short signedSample = (short)(sample * (short.MaxValue - short.MinValue) + short.MinValue);

                    int index = i * 2;

                    if (!BitConverter.IsLittleEndian)
                    {
                        audioBuffer[index] = (byte)(signedSample >> 8);
                        audioBuffer[index + 1] = (byte)signedSample;
                    }
                    else
                    {
                        audioBuffer[index] = (byte)signedSample;
                        audioBuffer[index + 1] = (byte)(signedSample >> 8);
                    }
                }

                sound.SubmitBuffer(audioBuffer);
            }           
        }

        public void SetScreenScale(int scale)
        {
            screenScale = scale;
            graphics.PreferredBackBufferWidth = screenWidth * screenScale;
            graphics.PreferredBackBufferHeight = screenHeight * screenScale;
            graphics.ApplyChanges();
        }

        public void SetVolume(float volume)
        {
            sound.Volume = volume;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            //Mono hanging fix
            if (Type.GetType("Mono.Runtime") != null)
            {
                Environment.FailFast(string.Empty);
            }

            //Actually exit
            Exit();
            Dispose();
        }
    }
}
