using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class IPServerPicker : IBalanceStrategy
    {
        public int numOfServers;

        public int determineServer(Socket client)
        {
            string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

            return Math.Abs(ip.GetHashCode()) % numOfServers;
        }

        public void updateBalanceData(int count)
        {
            numOfServers = count;
        }
    }
}
