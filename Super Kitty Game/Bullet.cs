using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Super_Kitty_Game
{
    public class Bullet : Sprite
    {
        private static Object bulletLock = new Object();
        public static List<Bullet> activeBullets = new List<Bullet>(), inactiveBullets = new List<Bullet>(), all = new List<Bullet>();
        
        const float speed = 500;
        private int index;
        private Cat owner; 

        private Bullet(Vector2 position, Cat cat): base(Game1.KittyTexture)
        {
            inactiveBullets.Add(this);
            Activate(position, cat);
            all.Add(this);
        }

        private void Activate(Vector2 position, Cat cat)
        {
            SetPosition(position);
            lock (bulletLock)
            {
                if (inactiveBullets.Remove(this))
                    activeBullets.Add(this);
                owner = cat;
                if (!owner.Bullets.Contains(this))
                {
                    index = owner.Bullets.Count;
                    owner.Bullets.Add(this);
                }
            }
        }

        private void Deactivate()
        {
            lock (bulletLock)
            {
                if (activeBullets.Remove(this))
                    inactiveBullets.Add(this);
                owner.Bullets.Remove(this);
                owner = null;
                index = -1;
            }
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            List<Bullet> aux = new List<Bullet>();
            lock (bulletLock)
            {
                foreach (Bullet b in activeBullets)
                {

                    if (b.Update(elapsedGameTime))
                    {
                        aux.Add(b);
                    }
                }
            }

            foreach (Bullet b in aux)
            {
                b.Deactivate();
            }
        }

        new private bool Update(float elapsedGameTime)
        {
            position.Y -= elapsedGameTime * speed;
            if (position.Y < -drawSize.Y)
                return true;
            return false;
        }

        public static void Shoot(Vector2 position, Cat cat)
        {
            lock (bulletLock)
            {
                if (inactiveBullets.Count != 0)
                {
                    Bullet b = inactiveBullets[0];
                    b.Activate(position, cat);
                }
                else
                    new Bullet(position, cat);
            }      
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            lock (bulletLock)
            {
                foreach (Bullet b in activeBullets)
                    b.Draw(sb, elapsedGameTime);
            }
        }

        public static void Write(BinaryWriter w, Cat cat)
        {
            lock (cat.BulletLock)
            {
                List<Bullet> bullets = cat.Bullets;
                w.Write((UInt16)bullets.Count);

                foreach (Bullet b in bullets)
                {
                    w.Write((UInt16)b.index);
                    w.Write((Int16)b.position.X);
                    w.Write((Int16)b.position.Y);
                }
            }
        }

        public static void Read(BinaryReader r, Cat cat)
        {
            int count = r.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                lock (cat.BulletLock)
                {
                    int index = r.ReadUInt16();
                    Vector2 position = new Vector2(r.ReadInt16(), r.ReadInt16());
                    while (cat.Bullets.Count <= index)
                    {
                        Shoot(position, cat);
                    }
                    cat.Bullets[index].Activate(position, cat);
                }
            }
        }
    }
}
