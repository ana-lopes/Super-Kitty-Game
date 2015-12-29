using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        private Rectangle boundingBox; //collider

        public SpriteEffects efeito = SpriteEffects.None;

        public Sprite(Texture2D texture)
        {
            sprite = texture;

            spriteSize = new Point(sprite.Width, sprite.Height);

            frameSize = new Point(texture.Width/6, texture.Height/2);

            boundingBox = new Rectangle((int)position.X, (int)position.Y, frameSize.X, frameSize.Y);

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

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

        public virtual Sprite SetPosition(Vector2 p)
        {
            this.position = p;            
            boundingBox.Location = new Point((int)position.X, (int)position.Y); //atualiza a posição do retangulo quando a sprite muda de posição

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

                if (currentFrame.X > 5)
                {
                    currentFrame.X = 0;
                }

                time = 0;
            }
            sb.Draw(sprite, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), color, 0, Vector2.Zero, 1, efeito, 0);
        }

    }
}