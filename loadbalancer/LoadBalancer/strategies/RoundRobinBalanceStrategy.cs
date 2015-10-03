using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class RoundRobinServerPicker : IBalanceStrategy
    {
        public int numOfServers;

        int hop = 0;

        public int determineServer(Socket client)
        {
            return hop++ % numOfServers;
        }

        public void updateBalanceData(int count)
        {
            numOfServers = count;
        }
    }
}
