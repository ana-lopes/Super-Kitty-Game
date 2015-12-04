using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Net;

namespace Super_Kitty_Game
{
    public class Cat : Sprite
    {
        public IPEndPoint endpoint;
        public Cat(string ip, int port) : base(Game1.kittyTexture)
        {
            this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public Cat(IPEndPoint ep) : base(Game1.kittyTexture)
        {
            this.endpoint = ep;
        }
    }
}
