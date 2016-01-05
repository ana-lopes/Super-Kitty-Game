using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Kitty_Game
{
    public class MasterClient : MyUDPClient
    {
        KeyboardState ksa;
        public MasterClient(int port)
            : base(port, Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString())
        {
            registered = true;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.R) && !ksa.IsKeyDown(Keys.R))
            {
                SendReset();
                Enemy.Reset();
                end = false; 
            }
            else
            {
                sendTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (startTimer >= startTime)
                    PhysicsWorld.CheckColiision();
                else
                    startTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                PhysicsWorld.UpdateAll((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (sendTimer >= sendInterval)
                {
                    foreach (IPEndPoint ep in Cats.Keys)
                    {
                        if (ep.ToString() != Cats.First().Key.ToString())
                            SendPosition(ep);
                    }
                    SendPositions(); 
                    sendTimer = 0;
                }
                BeginReceive(Receive, null);
            }
            ksa = ks;
        }

        private void SendPositions()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(positionsByte);
            lock (CatsLock)
            {
                w.Write((UInt16)Cats.Count);
                foreach (Cat cat in Cats.Values)
                {
                    byte[] ipBytes = cat.EndPoint.Address.GetAddressBytes();
                    w.Write(ipBytes);
                    w.Write((UInt16)cat.EndPoint.Port);
                    /*Vector2 position = cat.Position;
                    w.Write((Int16)position.X);
                    w.Write((Int16)position.Y);
                    w.Write((Int16)cat.efeito);
                    w.Write((Int16)cat.Speed);
                    Bullet.Write(w, cat);*/
                    Bullet.WriteOther(w, cat);
                }
                Enemy.Write(w);
                Console.WriteLine(w.BaseStream.Length);
                foreach (IPEndPoint ep in Cats.Keys)
                {
                    if (ep.ToString() != MasterEP.ToString())
                    {
                        try
                        {
                            Send(s.GetBuffer(), s.GetBuffer().Length, ep);
                        }
                        catch 
                        { 
                        }
                    }
                }
            }

            s.Dispose();
            w.Dispose(); 
        }

        private void SendReset()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(resetByte);
            foreach (IPEndPoint ep in Cats.Keys)
            {
                if (ep.ToString() != MasterEP.ToString())
                {
                    try
                    {
                        Send(s.GetBuffer(), s.GetBuffer().Length, ep);
                    }
                    catch { }
                }
            }
            s.Dispose();
            w.Dispose();
        }

        protected override void DecodeMessage(IPEndPoint remoteEP, byte[] receivedMSG)
        {
            base.DecodeMessage(remoteEP, receivedMSG);

            if (receivedMSG[0] == registryRequestByte)
            {
                ReceiveRequest(remoteEP, receivedMSG);
            }
        }

        private void ReceiveRequest(IPEndPoint remoteEP, byte[] receivedMSG)
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

            lock (CatsLock)
            {
                if (!Cats.ContainsKey(remoteEP))
                {
                    Cat newCat = new Cat(remoteEP);
                    Cats.Add(remoteEP, newCat);
                    newCat.SetPosition(new Vector2((float)x, (float)y));
                }
            }

            s2.Dispose();
            r.Dispose();
        }

             
    }
}
