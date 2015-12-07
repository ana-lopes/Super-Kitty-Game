using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

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
                SendPositions();
            }
            BeginReceive(Receive, null);
        }

        private void SendPositions()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(positionsByte);
            lock (catsLock)
            {
                w.Write((UInt16)cats.Count);
                foreach (Cat cat in cats.Values)
                {
                    byte[] ipBytes = cat.endpoint.Address.GetAddressBytes();
                    w.Write(ipBytes);
                    w.Write((UInt16)cat.endpoint.Port);
                    Vector2 position = cat.GetPosition();
                    w.Write((Int16)position.X);
                    w.Write((Int16)position.Y);
                    w.Write((Int16)cat.efeito);
                }

                foreach (IPEndPoint ep in cats.Keys)
                {
                    if (ep.ToString() != masterEP.ToString())
                    {
                        try
                        {
                            Send(s.GetBuffer(), s.GetBuffer().Length, ep);
                        }
                        catch { }
                    }
                }
            }

            s.Dispose();
            w.Dispose(); 
        }

        protected override void Receive(IAsyncResult result)
        {
            base.Receive(result);
            if (receivedMSG[0] == registryRequestByte)
            {
                ReceiveRequest();
            }
            else if (receivedMSG[0] == imHereByte)
            {
                ReceivePosition();
            }
        }

        private void ReceiveRequest()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(registryAcceptByte);
            Send(s.GetBuffer(), s.GetBuffer().Length, remoteEP);
            s.Dispose();
            w.Dispose();

            MemoryStream s2 = new MemoryStream(receivedMSG);
            BinaryReader r = new BinaryReader(s2);
            
            r.ReadByte();
            int x = r.ReadInt16();
            int y = r.ReadInt16();
            int efeito = r.ReadInt16();

            lock (catsLock)
            {
                if (!cats.ContainsKey(remoteEP))
                {
                    Cat newCat = new Cat(remoteEP);
                    cats.Add(remoteEP, newCat);
                    newCat.SetPosition(new Vector2((float)x, (float)y));
                }
            }

            s2.Dispose();
            r.Dispose();
        }

        private void ReceivePosition()
        {
            MemoryStream s = new MemoryStream(receivedMSG);
            BinaryReader r = new BinaryReader(s);

            r.ReadByte();
            int x = r.ReadInt16();
            int y = r.ReadInt16();
            int efeito = r.ReadInt16();

            lock (catsLock)
            {

                if (cats.ContainsKey(remoteEP))
                {
                    cats[remoteEP].SetPosition(new Vector2(x, y));
                    cats[remoteEP].efeito = (SpriteEffects)efeito;
                }
            }

            s.Dispose();
            r.Dispose();
        }
    }
}
