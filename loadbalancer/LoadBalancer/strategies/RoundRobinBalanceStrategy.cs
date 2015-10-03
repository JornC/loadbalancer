using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class RoundRobinBalanceStrategy : IBalanceStrategy
    {
        public int numOfServers;

        int hop = 0;

        public int determineServer(IInputStreamReadWriter client)
        {
            return hop++ % numOfServers;
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
