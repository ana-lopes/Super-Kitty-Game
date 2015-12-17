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

        protected const float accelleration = 20f;
        static protected Vector2 arenaSize;

        private List<Bullet> bullets;
        private Object bulletLock = new Object();


        public Cat(string ip, int port, Vector2 arena) : base(Game1.KittyTexture)
        {
            arenaSize = arena;
            this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            bullets = new List<Bullet>();
        }

        public Cat(IPEndPoint ep) : base(Game1.KittyTexture)
        {
            bullets = new List<Bullet>();
            this.endpoint = ep;
        }

        public override void Update(float elapsedGameTime)
        {
            velocity *= 0.98f;

            /*Vector2 direction = accelleration * Controller.GetDirection();
            velocity += direction;

            if (velocity.X > 0)
                efeito = SpriteEffects.FlipHorizontally;
            else
                efeito = SpriteEffects.None;*/

            Vector2 newPosition = Vector2.Clamp(
                elapsedGameTime * velocity + Position,
                Vector2.Zero, arenaSize - new Vector2(drawSize.X, drawSize.Y));
            SetPosition(newPosition);
        }

        public override void Draw(SpriteBatch sb, float elapsedGameTime)
        {
            base.Draw(sb, elapsedGameTime * velocity.Length() * 0.01f);
        }

        public float Speed
        {
            get 
            { 
                return velocity.X; 
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

        public List<Bullet> Bullets
        {
            get { return bullets; }
        }

        public Object BulletLock
        {
            get { return bulletLock; }
        }
    }
}
