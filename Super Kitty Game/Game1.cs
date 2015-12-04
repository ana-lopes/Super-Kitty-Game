using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Super_Kitty_Game
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D kittyTexture;

        Dictionary<IPEndPoint, Cat> cats = new Dictionary<IPEndPoint,Cat>();
        Player player;
        MyUDPClient client;

        public Game1(MyUDPClient client)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            this.client = client;
            Content.RootDirectory = "Content";
            this.client.cats = cats;
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
                , port, new Point(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            cats.Add(player.endpoint, player);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            kittyTexture = Content.Load<Texture2D>("kitty.png");
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            client.Update(gameTime);
            player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Cat cat in cats.Values)
                cat.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
