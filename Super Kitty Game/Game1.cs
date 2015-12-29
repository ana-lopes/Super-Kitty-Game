using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System;

namespace Super_Kitty_Game
{
    public class Game1 : Game
    {
        private static Game1 instance;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private static Texture2D kittyTexture;
        private static int arenaSizeX = 700, arenaSizeY = 600;

        private Dictionary<IPEndPoint, Cat> cats = new Dictionary<IPEndPoint,Cat>();
        private Player player;
        private MyUDPClient client;

        public Game1(MyUDPClient client)
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = arenaSizeX;
            graphics.PreferredBackBufferHeight = arenaSizeY;
            Content.RootDirectory = "Content";

            this.client = client;
            this.client.Cats = cats;
        }

        protected override void Initialize()
        {
            base.Initialize();
            int port = 0;
            if (client is MasterClient)
                port = MyUDPClient.MasterPort;
            else
                port = MyUDPClient.NormalPort;
            player = new Player(Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()
                , port, new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            cats.Add(player.EndPoint, player);

            if(client is MasterClient)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        new Enemy(new Vector2(50 + x * 60, y * 60));
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            kittyTexture = Content.Load<Texture2D>("kitty.png");
        }

        protected override void UnloadContent()
        {
            client.Close();
        }

        protected override void Update(GameTime gameTime)
        {
            client.Update(gameTime);
            player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            Enemy.UpdateAll((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            lock (client.CatsLock)
            {
                foreach (Cat cat in cats.Values)
                    cat.Draw(spriteBatch, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            Enemy.DrawAll(spriteBatch, (float)gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        static public Game1 Instance
        {
            get { return instance; }
        }

        static public Texture2D KittyTexture
        {
            get { return kittyTexture; }
        }

        static public Vector2 ArenaSize
        {
            get { return new Vector2(arenaSizeX, arenaSizeY); } //fix this
        }
    }
}
