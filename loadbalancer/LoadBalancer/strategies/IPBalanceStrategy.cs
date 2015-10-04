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
    class IPBalanceStrategy : SimpleDefaultStrategy 
    {

        public override int determineServer(IInputStreamReadWriter client)
        {
            string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

            return getKeyFromIndex(Math.Abs(ip.GetHashCode()) % Count);
        }

        public override int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
        {
            proxy = client;

            return determineServer(client);
        }
    }
}
