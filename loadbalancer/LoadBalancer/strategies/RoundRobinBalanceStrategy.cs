using LoadBalancer.strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class RoundRobinBalanceStrategy : SimpleDefaultStrategy
    {
        int hop = 0;

        public override int determineServer(IInputStreamReadWriter client)
        {
            if(Count == 0)
            {
                return 0;
            }

            return getKeyFromIndex(hop = ++hop % Count);
        }

        public override int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
        {
            proxy = client;

            return determineServer(client);
        }
    }
}
