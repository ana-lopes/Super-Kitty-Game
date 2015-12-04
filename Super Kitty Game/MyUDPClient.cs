using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using System.Net;
using System.IO;

namespace Super_Kitty_Game
{
    public class MyUDPClient : UdpClient
    {
        protected const float sendInterval = 0.1f;
        protected float sendTimer = 0;

        public const int MasterPort = 9999, NormalPort = 8888;
        protected const byte registryRequestByte = (byte)'M', registryAcceptByte = (byte)'L', positionsByte = (byte)'P';

        public Dictionary<IPEndPoint, Cat> cats;
        protected bool registered;
        protected IPEndPoint masterEP;
        protected byte[] receivedMSG;
        protected IPEndPoint remoteEndPoint;

        public MyUDPClient(int port, string masterIP)
            : base(port)
        {
            registered = false;
            Client.EnableBroadcast = true;
            masterEP = new IPEndPoint(IPAddress.Parse(masterIP), MasterPort);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!registered)
                GetRegistered();

            BeginReceive(Receive, null);
        }

        public void GetRegistered()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(registryRequestByte);
            Cat cat = cats.Values.First();
            Vector2 position = cat.GetPosition();
            w.Write((double)position.X);
            w.Write((double)position.Y);
            w.Write((UInt32)cat.efeito);

            Send(s.GetBuffer(), s.GetBuffer().Length, masterEP);
        }

        protected virtual void Receive(IAsyncResult result)
        {   
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            receivedMSG = EndReceive(result, ref remoteEndPoint); 
            
            Console.WriteLine("Received from " + remoteEndPoint.Address.ToString() + " with type " + (char)receivedMSG[0]);

            if (receivedMSG[0] == registryAcceptByte)
            {
                registered = true;
            }
            else if (receivedMSG[0] == positionsByte)
            {
                MemoryStream s = new MemoryStream(receivedMSG);
                BinaryReader r = new BinaryReader(s);

                r.ReadByte();
                uint count = r.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    IPAddress ip = new IPAddress(r.ReadBytes(4));
                    uint port = r.ReadUInt32();
                    IPEndPoint ep = new IPEndPoint(ip, (int)port);
                    Vector2 position = new Vector2((float)r.ReadDouble(), (float)r.ReadDouble());
                    if (!cats.ContainsKey(ep))
                    {
                        Cat newCat = new Cat(ep);
                        cats.Add(ep, newCat);
                        newCat.SetPosition(position);
                    }
                }

                s.Dispose();
                r.Dispose();
            }
        }
    }
}
