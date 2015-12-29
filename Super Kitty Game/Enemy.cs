using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Super_Kitty_Game
{
    public class Enemy : Sprite
    {
        public static List<Enemy> activeEnemies = new List<Enemy>(), inactiveEnemies = new List<Enemy>(), allEnemies = new List<Enemy>();



        enum Direction { left, right, down};
        private bool isDown = false, isRight = true;
        int index;
        private float timer = 0, moveNow = 0.1f; //move de 20 em 20 segundos

        bool hitRight = false, hitLeft = false;

        public Enemy(Vector2 position): base(Game1.KittyTexture)
        {
            allEnemies.Add(this);
            index = allEnemies.Count;
            SetPosition(position);
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            foreach (Enemy e in allEnemies)
                e.Draw(sb, elapsedGameTime);
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            foreach(Enemy e in allEnemies)
                e.Update(elapsedGameTime);
        }

        new private void Update(float elapsedGameTime)
        {
            timer += elapsedGameTime;

            if (timer >= moveNow)
            {
                if(!isDown)
                {
                    if (isRight)
                        SetPosition(new Vector2(position.X + 10, position.Y));

                    else if (!isRight)
                        SetPosition(new Vector2(position.X - 10, position.Y));

                    hitRight = position.X <= 0;
                    hitLeft = (position.X + drawSize.X) >= Game1.ArenaSize.X;

                    if(hitLeft || hitRight)
                    {
                        isRight = !isRight;
                        isDown = true;
                    }
                }

                else
                {
                    SetPosition(new Vector2(position.X, position.Y + 20));
                    isDown = false;
                }
                timer = 0;
            }
        }
        
        private void Deactivate()
        {
            if (activeEnemies.Remove(this))
                inactiveEnemies.Add(this);
        }

        public static void Write(BinaryWriter w, MyUDPClient client)
        {
            bool master = client is MasterClient;
            w.Write((UInt16)activeEnemies.Count);
            foreach (Enemy e in activeEnemies)
            {
                if (master)
                {
                    w.Write((UInt16)e.index);
                    w.Write((Int16)e.position.X);
                    w.Write((Int16)e.position.Y);
                }
            }
        }

        public static void Read(BinaryReader r)
        {
            int count = r.ReadUInt16();
            
        }

    }
}
