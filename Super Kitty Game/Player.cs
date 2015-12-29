
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Super_Kitty_Game
{
    public class Player : Cat
    {
        public Player(string ip, int port, Vector2 arenaSize) : base(ip, port, arenaSize)
        {           
            Random rnd = new Random();
            color = Color.Red;

            base.SetPosition(new Vector2(rnd.Next((int)arenaSize.X - base.drawSize.X) + base.drawSize.X,
                             (int)arenaSize.Y - base.drawSize.Y));

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

            if (Controller.Shoot(elapsedGameTime))
            {
                Bullet.Shoot(position, this);
            }
            
            Bullet.UpdateAll(elapsedGameTime);
        }

        public override void Draw(SpriteBatch sb, float elapsedGameTime)
        {
            base.Draw(sb, elapsedGameTime);
            Bullet.DrawAll(sb, elapsedGameTime);
        }
    }
}