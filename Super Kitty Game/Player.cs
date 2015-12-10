using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Super_Kitty_Game
{
    public class Player : Cat
    {
        private Vector2 arenaSize;
        private float accelleration = 20f;

        public Player(string ip, int port, Point arenaSize) : base(ip, port)
        {
            this.arenaSize = new Vector2(arenaSize.X, arenaSize.Y);
            Random rnd = new Random();
            color = Color.Red;

            base.SetPosition(rnd.Next(arenaSize.X - base.drawSize.X) + base.drawSize.X,
                             rnd.Next(arenaSize.Y - base.drawSize.Y) - base.drawSize.Y);
            velocity = new Vector2();
        }

        override public void Update(float elapsedGameTime)
        {
            velocity *= 0.98f;

            Vector2 direction = accelleration * Controller.GetDirection();
            velocity += direction;

            if (velocity.X > 0)
                efeito = SpriteEffects.FlipHorizontally;
            else
                efeito = SpriteEffects.None;

            Vector2 newPosition = Vector2.Clamp(
                elapsedGameTime * velocity + Position,
                Vector2.Zero, arenaSize - new Vector2(drawSize.X, drawSize.Y));
            SetPosition(newPosition);
        }
    }
}