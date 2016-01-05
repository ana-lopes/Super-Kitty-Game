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
        private static Object BulletLock = new Object();
        public static List<Bullet> all = new List<Bullet>();

        const float speed = 500;
        private Cat owner;

        private bool active;
        private bool sent;
        private int index;

        private Bullet(Vector2 position, Cat cat)
            : base(Game1.BulletTexture, 1, 1)
        {
            Activate(position);
            owner = cat;
            lock (owner.BulletLock)
            {
                index = cat.Bullets.Count;
                cat.Bullets.Add(this);
            }
            all.Add(this);
        }

        private void Activate(Vector2 position)
        {
            SetPosition(position);
            active = true;
            sent = false;
            this.Body.Activate();
        }

        public void Deactivate()
        {
            active = false;
            this.Body.Deactivate();
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            List<Bullet> aux = new List<Bullet>();
            lock (BulletLock)
            {
                foreach (Bullet b in all)
                {
                    if (b.active)
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
        }

        new private bool Update(float elapsedGameTime)
        {
            position.Y -= elapsedGameTime * speed;
            SetPosition(position);

            if (position.Y < -drawSize.Y)
                return true;
            return false;
        }

        public static void Shoot(Vector2 position, Cat cat)
        {
            lock (BulletLock)
            {
                bool recycled = false;
                foreach (Bullet b in cat.Bullets)
                {
                    if (!b.active)
                    {
                        b.Activate(position);
                        recycled = true;
                        break;
                    }
                }
                if (!recycled)
                    new Bullet(position, cat);
            }
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            lock (BulletLock)
            {
                foreach (Bullet b in all)
                {
                    if (b.active)
                        b.Draw(sb, elapsedGameTime);
                }
            }
        }

        public static void WriteOther(BinaryWriter w, Cat cat)
        {
            lock (cat.BulletLock)
            {
                List<Bullet> Bullets = cat.Bullets;
                w.Write((UInt16)Bullets.Count);

                foreach (Bullet b in Bullets)
                {
                    w.Write(Convert.ToUInt16(b.active));
                    w.Write(Convert.ToUInt16(!b.sent));
                    b.sent = true;
                }
            }
        }

        public static void WriteOwn(BinaryWriter w, Cat cat)
        {
            lock (cat.BulletLock)
            {
                List<Bullet> Bullets = cat.Bullets;
                w.Write((UInt16)Bullets.Count);

                foreach (Bullet b in Bullets)
                {
                    w.Write(Convert.ToUInt16(b.active));
                    w.Write(Convert.ToUInt16(!b.sent));                    
                    b.sent = true;
                    w.Write((Int16)b.position.X);
                    w.Write((Int16)b.position.Y);
                }
            }
        }

        public static void ReadOther(BinaryReader r, Cat cat)
        {
            int count = r.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                lock (cat.BulletLock)
                {
                    bool active = Convert.ToBoolean(r.ReadUInt16());
                    bool firstSent = Convert.ToBoolean(r.ReadUInt16());
                    Vector2 position = new Vector2(r.ReadInt16(), r.ReadInt16());

                    while (cat.Bullets.Count <= i)
                    {
                        Shoot(position, cat);                        
                    }
                    Bullet b = cat.Bullets[i];

                    if (firstSent)
                        b.Activate(position);
                    else
                        b.SetPosition(position);
                }
            }
        }

        public static void ReadOwn(BinaryReader r, Cat cat)
        {
            int count = r.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                bool active = Convert.ToBoolean(r.ReadUInt16());
                bool firstSent = Convert.ToBoolean(r.ReadUInt16());
                if (cat != null)
                {
                    lock (cat.BulletLock)
                    {
                        /*r.ReadInt16();
                        r.ReadInt16();*/

                        Bullet b = cat.Bullets[i];
                        if (!active && b.sent)
                            b.Deactivate();
                    }
                }
            }
        }

        public Cat Owner
        {
            get { return owner; }
        }
    }
}
