using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Super_Kitty_Game
{
    public static class Controller
    {
        static bool shooting;
        static float timer;
        const float cooldown = 0.5f;
        public static Vector2 GetDirection()
        {
            KeyboardState k = Keyboard.GetState();
            Vector2 v = new Vector2();
            
            if (k.IsKeyDown(Keys.Left))
                v -= Vector2.UnitX;
            if (k.IsKeyDown(Keys.Right))
                v += Vector2.UnitX;

            return v;
        }

        public static bool Shoot(float elapsedGameTime)
        {
            if (shooting)
            {
                timer += elapsedGameTime;
                if (timer > cooldown)
                {
                    shooting = false;
                }
            }

            if ((Mouse.GetState().LeftButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space)) && !shooting)
            {
                shooting = true;
                timer = 0;
                return true;
            }
            else return false;
        }
    }
}