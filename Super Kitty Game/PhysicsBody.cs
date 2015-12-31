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
        private bool active;


        public PhysicsBody(Rectangle tangle, Sprite owner)
        {
            this.tangle = tangle;
            this.owner = owner;
            Activate();
            PhysicsWorld.AddBody(this);
        }

        public void Activate()
        {
            active = true;
        }

        public void Deactivate()
        {
            active = false;
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

        public bool Active
        {
            get { return active; }
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
