﻿using Microsoft.Xna.Framework;
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
        private static Texture2D kittyTexture, bulletTexture, doggieTexture, spaceTexture;
        private static int arenaSizeX = 700, arenaSizeY = 600;

        private Dictionary<IPEndPoint, Cat> cats = new Dictionary<IPEndPoint,Cat>();
        private Player player;
        private MyUDPClient client;

        public Game1(MyUDPClient client)
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 600;
            this.IsMouseVisible = true;
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
                Enemy.Enemies();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            kittyTexture = Content.Load<Texture2D>("kitty.png");
            bulletTexture = Content.Load<Texture2D>("laser.png");
            doggieTexture = Content.Load<Texture2D>("dog (2).png");
            spaceTexture = Content.Load<Texture2D>("space.jpg");
        }

        protected override void UnloadContent()
        {
            client.Close();
        }

        protected override void Update(GameTime gameTime)
        {
            float elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lock (client.CatsLock)
            {
                foreach (var cat in cats.Values)
                {
                    cat.Update(elapsedGameTime);
                }
            }
            client.Update(gameTime);
            Bullet.UpdateAll(elapsedGameTime);

            if (client is MasterClient)
                Enemy.UpdateAll((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin();
            spriteBatch.Draw(spaceTexture, Vector2.Zero, new Rectangle(0, 0, spaceTexture.Width, spaceTexture.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            lock (client.CatsLock)
            {
                foreach (Cat cat in cats.Values)
                    cat.Draw(spriteBatch, elapsedGameTime);
            }
            Bullet.DrawAll(spriteBatch, elapsedGameTime);

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

        static public Texture2D BulletTexture
        {
            get { return bulletTexture; }
        }

        static public Texture2D DoggieTexture
        {
            get { return doggieTexture; }
        }

        static public Vector2 ArenaSize
        {
            get { return new Vector2(arenaSizeX, arenaSizeY); } //fix this
        }
    }
}
