using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Super_Kitty_Game
{
    public class Enemy : Sprite
    {
        public static List<Enemy> active = new List<Enemy>(), all = new List<Enemy>();
        static Random randEnemy = new Random();
        private static Object enemyLock = new Object();
        //static private float shootTimer = 0, shootNow = 1f;
        
        private bool isDown = false, isRight = true;
        private bool isActive;
        private float timer = 0, moveNow = 1f; //move de 20 em 20 segundos

        public static void Enemies()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    new Enemy(new Vector2(30 + x * 60, y * 60));
                }
            }
        }

        public Enemy(Vector2 position): base(Game1.KittyTexture, 6, 2)
        {
            Activate(position);
            all.Add(this);
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            lock(enemyLock)
            {
                foreach (Enemy e in active)
                    e.Draw(sb, elapsedGameTime);
            }
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            lock(enemyLock)
            {
                if (active.Count != 0)
                {
                    //int randomEnemy = randEnemy.Next(0, active.Count);
                    foreach (Enemy e in active)
                    {
                        e.Update(elapsedGameTime);
                    }
                    //active[randomEnemy].Shoot(elapsedGameTime);
                }
            }
        }

        public override void Collision(Sprite col)
        {
            if (col is Bullet)            
            {
                Bullet b = (Bullet)col;
                b.Deactivate();
                Deactivate();
            }
        }

        /*private void Shoot(float elapsedGameTime)
        {
            shootTimer += elapsedGameTime;

            if (shootTimer >= shootNow)
            {
                //Console.WriteLine("yello");
                shootTimer = 0;
            }
        }*/

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

                    bool hitRight = position.X <= 0;
                    bool hitLeft = (position.X + drawSize.X) >= Game1.ArenaSize.X;

                    if(hitLeft || hitRight)
                    {
                        isRight = !isRight;
                        isDown = true;

                        if (efeito == SpriteEffects.None)
                            efeito = SpriteEffects.FlipHorizontally;
                        else
                            efeito = SpriteEffects.None;
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
        
        private void Activate(Vector2 position)
        {
            SetPosition(position);
            if (!isActive)
            {
                isActive = true;
                active.Add(this);
            }
            this.Body.Activate();
            efeito = SpriteEffects.FlipHorizontally;
        }

        private void Deactivate()
        {
            isActive = false;
            active.Remove(this);
            this.Body.Deactivate();
            if (active.Count == 0)
            {
                MyUDPClient.Win();
            }
        }

        public static void Write(BinaryWriter w)
        {
            lock (enemyLock)
            {
                w.Write((UInt16)all.Count);
                foreach (Enemy e in all)
                {
                    w.Write(Convert.ToUInt16(e.isActive));
                    w.Write((Int16)e.position.X);
                    w.Write((Int16)e.position.Y);
                    w.Write(Convert.ToUInt16(e.isRight));
                    w.Write((Int16)e.efeito);
                }
            }
        }

        public static void Read(BinaryReader r)
        {
            lock (enemyLock)
            {
                int count = r.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    bool active = Convert.ToBoolean(r.ReadUInt16());
                    Vector2 position = new Vector2(r.ReadInt16(), r.ReadInt16());
                    bool isRight = Convert.ToBoolean(r.ReadUInt16());
                    SpriteEffects effect = (SpriteEffects)r.ReadInt16();

                    while (all.Count < count)
                    {
                        new Enemy(position);
                    }

                    Enemy e = all[i];
                    if (active)
                    {
                        e.Activate(position);
                        e.isRight = isRight;
                        e.efeito = effect;
                    }
                    else
                        e.Deactivate();
                }
            }
        }

        public static void Reset()
        {
            lock (enemyLock)
            {
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        Enemy e = all[y * 10 + x];
                        e.Activate(new Vector2(30 + x * 60, y * 60));
                        e.isRight = true;
                    }
                }
            }
        }
    }
}
