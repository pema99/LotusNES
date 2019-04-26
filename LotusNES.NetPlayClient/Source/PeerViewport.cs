using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.IO.Compression;

namespace LotusNES.NetPlayClient
{
    public class PeerViewport : Game
    {
        //Communication
        private TcpClient client;
        private NetworkStream clientStream;

        //Monogame stuff
        private GraphicsDeviceManager graphics;
        private SpriteBatch sb;
        private Texture2D blank;

        //Configuration
        private int screenWidth = 256;
        private int screenHeight = 240;

        //State
        private Color[] frameBuffer;
        private Texture2D frameBufferTexture;

        public PeerViewport()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.Window.AllowUserResizing = false;
        }

        protected override void Initialize()
        {
            string IP = Interaction.InputBox("Enter IP", "Lotus6502 NetPlay Peer");
            string Port = Interaction.InputBox("Enter Port", "Lotus6502 NetPlay Peer");
            client = new TcpClient(IP, int.Parse(Port));
            clientStream = client.GetStream();

            sb = new SpriteBatch(GraphicsDevice);
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new Color[] { Color.White });
            frameBuffer = new Color[screenWidth * screenHeight];
            frameBufferTexture = new Texture2D(GraphicsDevice, screenWidth, screenHeight);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            //Input
            KeyboardState KB = Keyboard.GetState();
            GamePadState GP = GamePad.GetState(PlayerIndex.One);
            bool[] inputs = new bool[] {
                KB.IsKeyDown(Keys.Z) || GP.IsButtonDown(Buttons.A) || GP.IsButtonDown(Buttons.LeftShoulder)  || GP.IsButtonDown(Buttons.LeftTrigger),
                KB.IsKeyDown(Keys.X) || GP.IsButtonDown(Buttons.B) || GP.IsButtonDown(Buttons.RightShoulder) || GP.IsButtonDown(Buttons.RightTrigger),

                KB.IsKeyDown(Keys.A) || GP.IsButtonDown(Buttons.Back),
                KB.IsKeyDown(Keys.S) || GP.IsButtonDown(Buttons.Start),

                KB.IsKeyDown(Keys.Up)    || GP.IsButtonDown(Buttons.LeftThumbstickUp)    || GP.IsButtonDown(Buttons.DPadUp),
                KB.IsKeyDown(Keys.Down)  || GP.IsButtonDown(Buttons.LeftThumbstickDown)  || GP.IsButtonDown(Buttons.DPadDown),
                KB.IsKeyDown(Keys.Left)  || GP.IsButtonDown(Buttons.LeftThumbstickLeft)  || GP.IsButtonDown(Buttons.DPadLeft),
                KB.IsKeyDown(Keys.Right) || GP.IsButtonDown(Buttons.LeftThumbstickRight) || GP.IsButtonDown(Buttons.DPadRight),
            };
            byte inputByte = 0;
            for (int i = 0; i < 8; i++)
            {
                inputByte |= (byte)((inputs[i] ? 1 : 0) << i);
            }
            clientStream.WriteByte(inputByte);

            //Draw frame
            while (clientStream.DataAvailable)
            {
                //Read length of framebuffer
                byte[] lengthBytes = new byte[4];
                clientStream.Read(lengthBytes, 0, 4);
                int length = BitConverter.ToInt32(lengthBytes, 0);

                //Read framebuffe-r
                byte[] compressedFrameBuffer = new byte[length];
                int totalBytesRead = 0;
                while (totalBytesRead < length)
                {
                    int bytesRead = clientStream.Read(compressedFrameBuffer, totalBytesRead, length - totalBytesRead);
                    totalBytesRead += bytesRead;
                }

                using (var source = new MemoryStream(compressedFrameBuffer))
                {
                    using (var dataStream = new GZipStream(source, CompressionMode.Decompress))
                    {
                        byte[] decompressedFrameBuffer = new byte[screenWidth * screenHeight];
                        dataStream.Read(decompressedFrameBuffer, 0, screenWidth * screenHeight);
                        for (int i = 0; i < screenWidth * screenHeight; i++)
                        {
                            frameBuffer[i] = PaletteMap.Map(decompressedFrameBuffer[i]);
                        }
                    }
                }
            }

            frameBufferTexture.SetData(frameBuffer);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sb.Begin();

            //FrameBuffer
            sb.Draw(frameBufferTexture, GraphicsDevice.Viewport.Bounds, Color.White);

            sb.End();

            base.Draw(gameTime);
        }
    }
}