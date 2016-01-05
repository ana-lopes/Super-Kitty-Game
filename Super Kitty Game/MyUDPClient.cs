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
        protected const byte imHereByte = (byte)'I', resetByte = (byte)'R';

        private Object catsLock = new Object();
        private Dictionary<IPEndPoint, Cat> cats;
        protected bool registered;
        private IPEndPoint masterEP;
        private bool exiting = false;
        protected float startTime = 2.0f, startTimer = 0;
                
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
                    foreach (IPEndPoint ep in cats.Keys)
                    {
                        if (ep.ToString() != cats.First().Key.ToString())
                            SendPosition(ep);
                    }
                    sendTimer = 0;
                }
            }
            
            try
            {
                BeginReceive(Receive, null);
            }
            catch
            {
                Exit("Connection lost");
            }
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

        public void SendPosition(IPEndPoint ep)
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
            Bullet.WriteOwn(w, cat);

            Send(s.GetBuffer(), s.GetBuffer().Length, ep);

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
            else if (receivedMSG[0] == imHereByte)
            {
                ReceivePosition(remoteEP, receivedMSG);
            }
            else if (receivedMSG[0] == resetByte)
            {
                ReceiveReset();
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
                /*int x = r.ReadInt16();
                int y = r.ReadInt16();
                SpriteEffects effect = (SpriteEffects)r.ReadInt16();
                int speed = r.ReadInt16();*/
                IPEndPoint ep = new IPEndPoint(ip, port);
                lock (catsLock)
                {
                    if (ep.ToString() != cats.First().Key.ToString())
                    {
                        //Vector2 position = new Vector2(x, y);
                        Cat cat;
                        if (!cats.TryGetValue(ep, out cat))
                        {
                            cat = new Cat(ep);
                            cats.Add(ep, cat);
                        }
                        Bullet.ReadOwn(r, cat);
                        /*cat.SetPosition(position);
                        cat.efeito = effect;
                        cat.Speed = speed;
                        Bullet.Read(r, cat, true);*/
                    }
                    else
                        Bullet.ReadOwn(r, cats.First().Value);
                }
            }
            try
            {
                Enemy.Read(r);
            }
            catch
            { }
                     

            s.Dispose();
            r.Dispose(); 
        }
        
        private void ReceivePosition(IPEndPoint remoteEP, byte[] receivedMSG)
        {
            MemoryStream s = new MemoryStream(receivedMSG);
            BinaryReader r = new BinaryReader(s);

            r.ReadByte();
            int x = r.ReadInt16();
            int y = r.ReadInt16();
            int efeito = r.ReadInt16();
            int speed = r.ReadInt16();

            lock (CatsLock)
            {
                Cat cat;
                if (Cats.TryGetValue(remoteEP, out cat))
                {
                    cat.SetPosition(new Vector2(x, y));
                    cat.efeito = (SpriteEffects)efeito;
                    cat.Speed = speed;
                    Bullet.ReadOther(r, cat);
                }
            }

            s.Dispose();
            r.Dispose();
        }   

        private void ReceiveReset()
        {
            end = false;
            Enemy.Reset();
        }

        protected static bool end = false;
        static public void Win()
        {
            if (!end)
            {
                end = true;
                MessageBox.Show("GG!, WP, MLG");
            }
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
