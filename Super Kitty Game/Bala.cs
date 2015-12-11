using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Kitty_Game
{
    class Bala : Sprite
    {
        public Bala(Texture2D texture, Vector2 position): base(Game1.KittyTexture)
        {
        }

        public override void Update(float elapsedGameTime)
        {
            base.Update(elapsedGameTime);
        }
    }
}
