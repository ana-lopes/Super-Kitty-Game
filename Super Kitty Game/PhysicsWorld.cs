using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Kitty_Game
{
    static class PhysicsWorld
    {
        static List<PhysicsBody> bodies = new List<PhysicsBody>();

        public static void CheckColiision()
        {
            for (int i = 0; i < bodies.Count - 1; i++)
            {
                for(int j = i + 1; j < bodies.Count; j++)
                {
                    if(bodies[i].Rec.Intersects(bodies[j].Rec))
                    {
                        //bodies[i].Owner.Collision(bodies[j].Owner);
                    }
                }
            }
        }

        public static void AddBody(PhysicsBody body)
        {
            bodies.Add(body);
        }

        public static bool RemoveBody(PhysicsBody body)
        {
            return bodies.Remove(body);
        }
    }
}
