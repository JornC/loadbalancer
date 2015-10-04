using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    interface IInputStreamReadWriter
    {
        EndPoint RemoteEndPoint { get; }

        int Receive(byte[] ba);

        void Feed(byte[] ba, int length);

        void Close();

        void Send(byte[] ba, int length, SocketFlags flags);

        Socket getSocket();
    }
}
