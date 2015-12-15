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
        public static List<Bullet> activeBullets = new List<Bullet>(), inactiveBullets = new List<Bullet>(), all = new List<Bullet>();
        
        const float speed = 500;
        public int index;
        private bool mine;

        private Bullet(Vector2 position, List<Bullet> active, List<Bullet> inactive, bool mine): base(Game1.KittyTexture)
        {
            inactiveBullets.Add(this);
            Activate(position, mine);
            index = all.Count;
            all.Add(this);
        }

        private void Activate(Vector2 position, bool mine)
        {
            SetPosition(position);
            if (inactiveBullets.Remove(this))
                activeBullets.Add(this);
            this.mine = mine;
        }

        private void Deactivate()
        {
            if (activeBullets.Remove(this))
                inactiveBullets.Add(this);
        }

        public static void UpdateAll(float elapsedGameTime)
        {
            List<Bullet> aux = new List<Bullet>();
            foreach (Bullet b in activeBullets)
            {

                if (b.Update(elapsedGameTime))
                {
                    aux.Add(b);
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

        public static void Shoot(Vector2 position, bool mine)
        {
            if (inactiveBullets.Count != 0)
            {
                Bullet b = inactiveBullets[0];
                b.Activate(position, mine);
            }
            else
                new Bullet(position, activeBullets, inactiveBullets, mine);        
        }

        public static void DrawAll(SpriteBatch sb, float elapsedGameTime)
        {
            foreach (Bullet b in activeBullets)
                b.Draw(sb, elapsedGameTime);
        }

        public static void Write(BinaryWriter w, MyUDPClient client)
        {
            bool master = client is MasterClient;
            w.Write((UInt16)activeBullets.Count);
            foreach (Bullet b in activeBullets)
            {
                if (master || b.mine)
                {
                    w.Write((UInt16)b.index);
                    w.Write((Int16)b.position.X);
                    w.Write((Int16)b.position.Y);
                }
            }
        }

        public static void Read(BinaryReader r)
        {
            int count = r.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                int index = r.ReadUInt16();
                Vector2 position = new Vector2(r.ReadInt16(), r.ReadInt16());
                while (all.Count <= index)
                {
                    Shoot(position, false);
                }
                all[index].Activate(position, false);
            }
        }

        public override Sprite SetPosition(int x, int y)
        {
            if (mine)
                return null;
            else
                return base.SetPosition(x, y);
        }
    }
}
