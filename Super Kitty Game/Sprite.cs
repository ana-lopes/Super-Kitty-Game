using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Super_Kitty_Game
{
    public class Sprite
    {
        private Texture2D sprite;
        private Point spriteSize;
        protected Point drawSize = new Point(300, 300);
        public Vector2 position = new Vector2(400, 300);
        protected Color color = Color.White;

        private Point currentFrame = new Point(0, 0);
        private Point frameSize;
        private float frameDuration = 0.1f;
        private float time = 0;
        
        private PhysicsBody body;

        int columns, rows;

        static public List<Bullet> auxBullets = new List<Bullet>();

        public SpriteEffects efeito = SpriteEffects.None;

        public Sprite(Texture2D texture, int columns, int rows)
        {
            sprite = texture;

            spriteSize = new Point(sprite.Width, sprite.Height);

            frameSize = new Point(texture.Width/columns, texture.Height/rows);

            body = new PhysicsBody(new Rectangle((int)position.X, (int)position.Y, frameSize.X, frameSize.Y), this);

            this.columns = columns;
            this.rows = rows;

            computeHeight();

            drawSize = frameSize;
        }

        public Vector2 Position
        {
            get { return position; }
        }

        private void computeHeight()
        {
            drawSize.Y = (int)((float)drawSize.X * (float)spriteSize.Y / (float)spriteSize.X);
        }

        public virtual void Update(float elapsedGameTime)
        {
            
        }

        virtual public void Collision(Sprite col)
        {
        }

        public virtual Sprite SetPosition(Vector2 p)
        {
            this.position = p;

            return this;
        }

        protected Vector2 toPoint(Vector2 p)
        {
            return new Vector2((0.5f + p.X), (0.5f + p.Y));
        }

        public virtual Sprite SetDrawWidth(int width)
        {
            drawSize.X = width;
            computeHeight();
            return this;
        }

        public virtual void Draw(SpriteBatch sb, float elapsedGameTime)
        {
            time += elapsedGameTime;

            if (time >= frameDuration)
            {
                currentFrame.X++;

                if (currentFrame.X > columns-1)
                {
                    currentFrame.X = 0;
                }

                time = 0;
            }
            sb.Draw(sprite, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), color, 0, Vector2.Zero, 1, efeito, 0);
        }

       protected float FrameSizeX
        {
            get { return frameSize.X; }
        }

        protected PhysicsBody Body
        {
            get { return body; }
        }
    }
}