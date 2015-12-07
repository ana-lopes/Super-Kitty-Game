using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using System.Net;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Super_Kitty_Game
{
    public class MyUDPClient : UdpClient
    {
        protected const float sendInterval = 0.1f;
        protected float sendTimer = 0;

        public const int MasterPort = 9999, NormalPort = 8888;
        protected const byte registryRequestByte = (byte)'M', registryAcceptByte = (byte)'L', positionsByte = (byte)'P';
        protected const byte imHereByte = (byte)'I';

        public Object catsLock = new Object();
        public Dictionary<IPEndPoint, Cat> cats;
        protected bool registered;
        protected IPEndPoint masterEP;
                
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
                SendRequest();
            else
            {
                sendTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (sendTimer >= sendInterval)
                {
                    SendPosition();
                }
            }
            
            BeginReceive(Receive, null);
        }

        private void SendRequest()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(registryRequestByte);
            Cat cat;
            lock (catsLock)
            {
                cat = cats.Values.First();
            }
            Vector2 position = cat.GetPosition();
            w.Write((Int16)position.X);
            w.Write((Int16)position.Y);
            w.Write((Int16)cat.efeito);

            Send(s.GetBuffer(), s.GetBuffer().Length, masterEP);

            s.Dispose();
            w.Dispose();
        }

        private void SendPosition()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(imHereByte);
            Cat cat;
            lock (catsLock)
            {
                cat = cats.Values.First();
            }
            Vector2 position = cat.GetPosition();
            w.Write((Int16)position.X);
            w.Write((Int16)position.Y);
            w.Write((Int16)cat.efeito);

            Send(s.GetBuffer(), s.GetBuffer().Length, masterEP);

            s.Dispose();
            w.Dispose();
        }

        protected virtual void Receive(IAsyncResult result)
        {   
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedMSG = EndReceive(result, ref remoteEP);

                Console.WriteLine("Received from " + remoteEP.Address.ToString() + " with type " + (char)receivedMSG[0]);

                if (receivedMSG[0] == registryAcceptByte)
                {
                    ReceiveRegistry();
                }
                else if (receivedMSG[0] == positionsByte)
                {
                    ReceivePositions(receivedMSG);
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show("Connection lost");
                Game1.instance.Exit();
            }
        }

        private void ReceiveRegistry()
        {
            registered = true;
        }

        private void ReceivePositions(byte[] receivedMSG)
        {
            MemoryStream s = new MemoryStream(receivedMSG);
            BinaryReader r = new BinaryReader(s);

            r.ReadByte();
            int count = r.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                IPAddress ip = new IPAddress(r.ReadBytes(4));
                int port = r.ReadUInt16();
                IPEndPoint ep = new IPEndPoint(ip, port);
                lock (catsLock)
                {
                    if (ep.ToString() != cats.First().Key.ToString())
                    {
                        Vector2 position = new Vector2(r.ReadInt16(), r.ReadInt16());
                        if (!cats.ContainsKey(ep))
                        {
                            Cat newCat = new Cat(ep);
                            cats.Add(ep, newCat);
                        }
                        cats[ep].SetPosition(position);
                        cats[ep].efeito = (SpriteEffects)r.ReadInt16();
                    }
                }
            }

            s.Dispose();
            r.Dispose(); 
        }
    }
}
