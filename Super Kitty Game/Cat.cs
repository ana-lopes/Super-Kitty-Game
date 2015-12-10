using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Net;
using Microsoft.Xna.Framework;

namespace Super_Kitty_Game
{
    public class Cat : Sprite
    {
        protected Vector2 velocity;
        private IPEndPoint endpoint;

        public Cat(string ip, int port) : base(Game1.KittyTexture)
        {
            this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public Cat(IPEndPoint ep) : base(Game1.KittyTexture)
        {
            this.endpoint = ep;
        }

        public override void Draw(SpriteBatch sb, float elapsedGameTime)
        {
            base.Draw(sb, elapsedGameTime * velocity.Length() * 0.01f);
        }

        public float Speed
        {
            get 
            { 
                return velocity.Length(); 
            }
            set 
            { 
                velocity = Vector2.UnitX * value; 
            }
        }

        public IPEndPoint EndPoint
        {
            get { return endpoint; }
        }
    }
}
