using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Super_Kitty_Game
{
    public static class Controller
    {
        public static Vector2 GetDirection()
        {
            KeyboardState k = Keyboard.GetState();
            Vector2 v = new Vector2();

            if (k.IsKeyDown(Keys.Down))
                v += Vector2.UnitY;
            if (k.IsKeyDown(Keys.Up))
                v -= Vector2.UnitY;
            if (k.IsKeyDown(Keys.Left))
                v -= Vector2.UnitX;
            if (k.IsKeyDown(Keys.Right))
                v += Vector2.UnitX;

            return v;
        }
    }
}