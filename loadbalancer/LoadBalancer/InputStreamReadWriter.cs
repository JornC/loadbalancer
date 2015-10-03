using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class InputStreamReadWriter
    {
        List<byte[]> buffers = new List<byte[]>();

        public int Receive(byte[] ba)
        {
            return 0;
        }

        public void Feed(byte[] ba)
        {

        }

        public static InputStreamReadWriter Wrap(Socket sock)
        {
            return null;
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
