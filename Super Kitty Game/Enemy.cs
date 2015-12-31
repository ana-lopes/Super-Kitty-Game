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

        private static Object enemyLock = new Object();

        private bool isDown = false, isRight = true;
        public int index;
        private float timer = 0, moveNow = 1f; //move de 20 em 20 segundos

        static private float shootTimer = 0, shootNow = 1f;

        bool hitRight = false, hitLeft = false;
        Rectangle rec;

        static Random randEnemy = new Random();
        static int randomEnemy;

        public Enemy(Vector2 position): base(Game1.KittyTexture, 6, 2)
        {
            allEnemies.Add(this);
            activeEnemies.Add(this);
            index = allEnemies.Count;
            SetPosition(position);

            efeito = SpriteEffects.FlipHorizontally;
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            lock(enemyLock)
            {
                foreach (Enemy e in activeEnemies)
                    e.Draw(sb, elapsedGameTime);
            }
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            lock(enemyLock)
            {
                randomEnemy = randEnemy.Next(0, activeEnemies.Count);
                foreach (Enemy e in activeEnemies)
                {
                    e.Update(elapsedGameTime);
                    if (e.index == randomEnemy)
                        e.Shoot(elapsedGameTime);
                }
            }
        }

        private void Shoot(float elapsedGameTime)
        {
            shootTimer += elapsedGameTime;

            if (shootTimer >= shootNow)
            {
                Console.WriteLine("yello");
                shootTimer = 0;
            }
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
        
        private void Deactivate()
        {
            lock(enemyLock)
            {
                if (activeEnemies.Remove(this))
                    inactiveEnemies.Add(this);
            }
        }

        public static void Write(BinaryWriter w, MyUDPClient client)
        {
            
            w.Write((UInt16)activeEnemies.Count);
            foreach (Enemy e in activeEnemies)
            {
                w.Write((UInt16)e.index);
                w.Write((Int16)e.position.X);
                w.Write((Int16)e.position.Y);
            }
        }

        public static void Read(BinaryReader r)
        {
            int count = r.ReadUInt16();
        }

    }
}
