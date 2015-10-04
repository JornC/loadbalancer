using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class SocketInputStreamReadWriter : IInputStreamReadWriter
    {
        private Socket sock;

        public EndPoint RemoteEndPoint
        {
            get
            {
                return sock.RemoteEndPoint;
            }
        }

        public SocketInputStreamReadWriter()
        {

        }

        public SocketInputStreamReadWriter(Socket sock)
        {
            Wrap(sock);
        }

        public int Receive(byte[] ba)
        {
            return sock.Receive(ba);
        }

        public void Feed(byte[] ba, int length)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            sock.Close();
        }

        public void Send(byte[] ba, int length, SocketFlags flags)
        {
            sock.Send(ba, length, flags);
        }

        public Socket getSocket()
        {
            return sock;
        }

        public void Wrap(Socket sock)
        {
            this.sock = sock;
        }
    }
}
