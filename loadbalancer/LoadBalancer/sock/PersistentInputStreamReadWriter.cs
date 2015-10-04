using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer.sock
{
    
    class PersistentInputStreamReadWriter : IInputStreamReadWriter
    {
        private Socket sock;
        private SessionPersistentBalanceStrategy strategy;
        private int servNum;

        public EndPoint RemoteEndPoint
        {
            get
            {
                return sock.RemoteEndPoint;
            }
        }

        public PersistentInputStreamReadWriter(SessionPersistentBalanceStrategy strategy, int servNum)
        {
            this.strategy = strategy;
            this.servNum = servNum;
        }

        public void Close()
        {
            sock.Close();
        }

        public void Feed(byte[] ba, int length)
        {
            throw new NotImplementedException();
        }

        public Socket getSocket()
        {
            return sock;
        }

        public int Receive(byte[] ba)
        {
            int length = sock.Receive(ba);

            string content = Encoding.ASCII.GetString(ba);

            int idx;
            if ((idx = content.IndexOf("set-cookie:")) != -1)
            {
                int endIdx = content.IndexOf(';', idx);

                String firstCookie = content.Substring(idx, endIdx - idx);
                String sessionID = firstCookie.Split('=').Skip(1).First();

                if (sessionID != null)
                {
                    Console.WriteLine("Session initialized ({0}) in server response, notifying strategy.", sessionID);
                    strategy.InsertSession(sessionID, servNum);
                }
            }

            return length;
        }

        public void Send(byte[] ba, int length, SocketFlags flags)
        {
            sock.Send(ba, length, flags);
        }

        public void Wrap(Socket sock)
        {
            this.sock = sock;
        }
    }
}
