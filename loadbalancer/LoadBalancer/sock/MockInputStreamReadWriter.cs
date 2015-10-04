using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class MockInputStreamReadWriter : IInputStreamReadWriter
    {

        List<byte[]> buffers = new List<byte[]>();
        private Socket sock;

        public EndPoint RemoteEndPoint { get
            {
                return sock.RemoteEndPoint;
            }
        }

        public MockInputStreamReadWriter(Socket sock)
        {
            this.sock = sock;
        }

        public void Close()
        {
            sock.Close();
        }

        public void Feed(byte[] ba, int length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(ba, buffer, length);
            buffers.Add(buffer);
        }

        public int Receive(byte[] ba)
        {
            int cnt = 0;

            foreach (byte[] buffer in buffers)
            {
                int consume = Math.Min(ba.Length - cnt, buffer.Length);

                Array.Copy(buffer, 0, ba, cnt, consume);
                cnt += buffer.Length;
                /*
                buffers.Remove(buffer);
                if (consume != buffer.Length)
                {
                    byte[] leftOvers = new byte[buffer.Length - consume];
                    Array.Copy(buffer, consume, leftOvers, 0, leftOvers.Length);
                    buffers.Insert(0, leftOvers);
                }
                */

                if (cnt > ba.Length)
                {
                    break;
                }
            }

            return cnt;
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
            throw new NotImplementedException();
        }
    }
}
