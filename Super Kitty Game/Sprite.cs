using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Kitty_Game
{
    public class Sprite
    {
        Texture2D sprite;
        Point SpriteSize;
        protected Point DrawSize = new Point(300, 300);
        protected Vector2 position = new Vector2(400, 300);
        protected Color color = Color.White;

        Point currentFrame = new Point(0, 0);
        Point frameSize;
        float frameDuration = 0.1f;
        float time = 0;

        public SpriteEffects efeito = SpriteEffects.None;

        public Sprite(Texture2D texture)
        {
            sprite = texture;

            SpriteSize = new Point(sprite.Width, sprite.Height);

            frameSize = new Point(texture.Width/6, texture.Height/2);

            computeHeight();

            DrawSize = frameSize;
        }

        public Vector2 GetPosition()
        {
            return new Vector2((float)position.X, (float)position.Y);
        }
        private void computeHeight()
        {
            DrawSize.Y = (int)((float)DrawSize.X * (float)SpriteSize.Y / (float)SpriteSize.X);
        }

        public virtual void Update(float elapsedGameTime)
        {
            //time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += elapsedGameTime;

            if(time >= frameDuration)
            {
                currentFrame.X++;

                if(currentFrame.X > 5)
                {
                    currentFrame.X = 0;
                }

                time = 0;
            }
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

        public virtual Sprite SetPosition(int x, int y)
        {
            return this.SetPosition(new Vector2(x, y));
        }
        public virtual Sprite SetDrawWidth(int width)
        {
            DrawSize.X = width;
            computeHeight();
            return this;
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Draw(sprite, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), color, 0, Vector2.Zero, 1, efeito, 1);
        }

    }
}