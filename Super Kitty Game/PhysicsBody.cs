using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Super_Kitty_Game
{
    public class PhysicsBody : IDisposable
    {
        Rectangle tangle;
        Sprite owner;

        public PhysicsBody(Rectangle tangle, Sprite owner)
        {
            this.tangle = tangle;
            this.owner = owner;
            PhysicsWorld.AddBody(this);
        }

        public virtual void Update(float elapsedGameTime)
        {
            tangle.Location = new Point((int)owner.position.X, (int)owner.position.Y);
        }

        bool disposed = false;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if(disposing)
            {
                PhysicsWorld.RemoveBody(this);
            }

            disposed = true;
        }

        ~PhysicsBody()
        {
            Dispose(false);
        }

        public Rectangle Rec
        {
            get { return tangle; }
        }

        public Sprite Owner
        {
            get { return owner; }
        }
    }
}
