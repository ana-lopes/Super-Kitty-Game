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

        private Object catsLock = new Object();
        private Dictionary<IPEndPoint, Cat> cats;
        protected bool registered;
        private IPEndPoint masterEP;
        private bool exiting = false;
                
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
                    sendTimer = 0;
                }
            }
            /*
            try
            {*/
                BeginReceive(Receive, null);
            /*}
            catch
            {
                Exit("Connection lost");
            }*/
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
            Vector2 position = cat.Position;
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
            Vector2 position = cat.Position;
            w.Write((Int16)position.X);
            w.Write((Int16)position.Y);
            w.Write((Int16)cat.efeito);
            w.Write((Int16)cat.Speed);

            Send(s.GetBuffer(), s.GetBuffer().Length, masterEP);

            s.Dispose();
            w.Dispose();
        }

        protected void Receive(IAsyncResult result)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedMSG = EndReceive(result, ref remoteEP);

                Console.WriteLine("Received from " + remoteEP.Address.ToString() + " with type " + (char)receivedMSG[0]);

                DecodeMessage(remoteEP, receivedMSG);
            }
            catch (SocketException e)
            {
                Exit("Connection lost");
            }
            catch (ObjectDisposedException e)
            { }
        }

        protected void Exit(string reason)
        { 
            if (!exiting)
            {
                exiting = true;
                MessageBox.Show(reason);
                Game1.Instance.Exit();
            }
        }

        protected virtual void DecodeMessage(IPEndPoint remoteEP, byte[] receivedMSG)
        {
            if (receivedMSG[0] == registryAcceptByte)
            {
                ReceiveRegistry();
            }
            else if (receivedMSG[0] == positionsByte)
            {
                ReceivePositions(receivedMSG);
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
                int x = r.ReadInt16();
                int y = r.ReadInt16();
                SpriteEffects effect = (SpriteEffects)r.ReadInt16();
                int speed = r.ReadInt16();
                IPEndPoint ep = new IPEndPoint(ip, port);
                lock (catsLock)
                {
                    if (ep.ToString() != cats.First().Key.ToString())
                    {
                        Vector2 position = new Vector2(x, y);
                        Cat cat;
                        if (!cats.TryGetValue(ep, out cat))
                        {
                            cat = new Cat(ep);
                            cats.Add(ep, cat);
                        }
                        cat.SetPosition(position);
                        cat.efeito = effect;
                        cat.Speed = speed;
                    }
                }
            }

            Bullet.Read(r);                      

            s.Dispose();
            r.Dispose(); 
        }        

        public Object CatsLock
        {
            get { return catsLock; }
        }

        public Dictionary<IPEndPoint, Cat> Cats
        {
            get { return cats; }
            set
            {
                if (cats == null)
                    cats = value;
            }
        }

        public IPEndPoint MasterEP
        {
            get { return masterEP; }
        }
    }
}
