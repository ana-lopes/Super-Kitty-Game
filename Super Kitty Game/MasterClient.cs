using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Super_Kitty_Game
{
    public class MasterClient : MyUDPClient
    {
        public MasterClient(int port)
            : base(port, Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString())
        {
            registered = true;
        }

        public override void Update(GameTime gameTime)
        {
            sendTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (sendTimer >= sendInterval)
            {
                SendInfo();
            }
            BeginReceive(Receive, null);
        }

        private void SendInfo()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(positionsByte);
            w.Write((UInt32)cats.Count);
            foreach (Cat cat in cats.Values)
            {
                byte[] ipBytes = cat.endpoint.Address.GetAddressBytes();
                w.Write(ipBytes);
                w.Write((UInt32)cat.endpoint.Port);
                Vector2 position = cat.GetPosition();
                w.Write((double)position.X);
                w.Write(position.Y);
            }

            foreach (IPEndPoint ep in cats.Keys)
            {
                Send(s.GetBuffer(), s.GetBuffer().Length, ep);
            }

            s.Dispose();
            w.Dispose(); 
        }

        protected override void Receive(IAsyncResult result)
        {
            base.Receive(result);
            if (receivedMSG[0] == registryRequestByte)
            {
                Register();
            }
        }

        private void Register()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(registryAcceptByte);
            Send(s.GetBuffer(), s.GetBuffer().Length, remoteEndPoint);
            s.Dispose();
            w.Dispose();
        }
    }
}
