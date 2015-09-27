using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class IPServerPicker : IServerPickert
    {
        public int serverMaxCons;
        public int numOfServers;
        public double servConv;

        public IPServerPicker()
        {
            
        }

        public int determineServer(Socket client)
        {
            string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
            ip = ip.Replace(".", "");
            double last = Double.Parse(ip.Substring(ip.Length - serverMaxCons, serverMaxCons));

            return (int)((last / numOfServers) * (numOfServers * servConv));
        }

        public void updateBalanceData(int count)
        {
            numOfServers = count;
            serverMaxCons = numOfServers.ToString().Length;
            servConv = numOfServers / Math.Pow(10, (double)serverMaxCons);
        }
    }
}
