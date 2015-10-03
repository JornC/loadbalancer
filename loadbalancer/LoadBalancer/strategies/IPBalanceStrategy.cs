using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class IPBalanceStrategy : IBalanceStrategy
    {
        public int numOfServers;

        public int determineServer(IInputStreamReadWriter client)
        {
            string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

            return Math.Abs(ip.GetHashCode()) % numOfServers;
        }

        public int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
        {
            proxy = client;

            return determineServer(client);
        }

        public void updateBalanceData(int count)
        {
            numOfServers = count;
        }
    }
}
