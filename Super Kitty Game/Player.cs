using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Super_Kitty_Game
{
    public class Player : Cat
    {
        Vector2 velocity;
        Vector2 arenaSize;
        float accelleration = 20f;

        public Player(string ip, int port, Point arenaSize) : base(ip, port)
        {
            this.arenaSize = new Vector2(arenaSize.X, arenaSize.Y);
            Random rnd = new Random();
            color = Color.Red;

            base.SetPosition(rnd.Next(arenaSize.X - base.DrawSize.X) + base.DrawSize.X,
                             rnd.Next(arenaSize.Y - base.DrawSize.Y) - base.DrawSize.Y);
            velocity = new Vector2();
        }

        private Vector2 Clamp(Vector2 p, Vector2 minimum, Vector2 maximum)
        {
            p.X = p.X < minimum.X ? minimum.X : (p.X > maximum.X ? maximum.X : p.X);
            p.Y = p.Y < minimum.Y ? minimum.Y : (p.Y > maximum.Y ? maximum.Y : p.Y);
            return p;
        }

        override public void Update(GameTime gameTime)
        {
            velocity *= 0.98f;

            Vector2 direction = accelleration * Controller.GetDirection();
            velocity += direction;

            if (velocity.X > 0)
                efeito = SpriteEffects.FlipHorizontally;
            else
                efeito = SpriteEffects.None;

            Vector2 newPosition = Clamp(
                (float)gameTime.ElapsedGameTime.TotalSeconds * velocity + GetPosition(),
                Vector2.Zero, arenaSize - new Vector2(DrawSize.X, DrawSize.Y));
            SetPosition(newPosition);

            base.Update(gameTime);
        }

    }
}