using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Loadbalancer
{
    class Balancer
    {
        public List<Clients> listofclients { get; set; }
        public List<Server> listofservers { get; set; }

        public Balancer()
        {
            listofservers = ServerController.Instance.listofservers;
            listofclients = new List<Clients>();
            initializeServerList();
        }

        private void initializeServerList()
        {
            Thread thread = new Thread(updateServerList);
            thread.Start();
        }

        private void updateServerList()
        {
            while (true)
            {
                bool serveroffline;

                for (int i = 0; i < listofclients.Count; i++)
                {
                    Clients client = listofclients[i];
                    serveroffline = true;

                    //check if client is still online
                    if (client.lasttimeonline.CompareTo(DateTime.Now.AddMinutes(-1)) <= 0)
                    {
                        listofclients.Remove(client);
                    }

                    for (int n = 0; n < listofservers.Count; n++)
                    {
                        Server server = listofservers[n];
                        //check if client is connected to online server
                        if (server.name.Equals(client.server.name))
                        {
                            serveroffline = false;
                        }
                    }
                    //remove client if connected to an offline server
                    if (serveroffline)
                    {
                        listofclients.Remove(client);
                    }
                }
            }
        }

        public Server checkClient(string ip)
        {
            Clients client = listofclients.FirstOrDefault(xclient => xclient.ip.Equals(ip));
            if (client != null)
            {
                return client.server;
            }
            return null;
        }

        public void clientServerBinding(string ip, Server server)
        {
            Clients client = new Clients(server, ip);
            listofclients.Add(client);
        }
    }

    class Clients
    {
        public Server server;
        public string ip;
        public DateTime lasttimeonline;

        public Clients(Server server, string ip)
        {
            this.server = server;
            this.ip = ip;
            lasttimeonline = DateTime.Now;
        }
    }

    class RandomBalancing : Balancer
    {
        private int counter;

        public RandomBalancing() : base()
        {
            counter = 1;
        }

        public Server getServer(string ip)
        {
            //return pickOtherServer();
            Server server = checkClient(ip);
            
            if (server != null)
            {
                return server;
            }
            else
            {
                server = pickOtherServer();
                clientServerBinding(ip, server);
                return server;
            }
        }

        public Server pickOtherServer()
        {
            int temporary = RandomNumber(0, listofservers.Count);
            return listofservers[temporary];
        }

        //Thread synchronization called synclock
        private static readonly Random random = new Random();
        private static readonly object synclock = new object();

        public static int RandomNumber(int minimum, int maximum)
        {
            lock (synclock)
            {
                return random.Next(minimum, maximum);
            }
        }
    }

    class RoundRobin : Balancer
    {
        private int counter;

        public RoundRobin() : base()
        {
            counter = 1;
        }

        public Server getServer(string ip)
        {
            //return pickOtherServer();
            Server server = checkClient(ip);

            if (server != null)
            {
                return server;
            }
            else
            {
                server = pickOtherServer();
                clientServerBinding(ip, server);
                return server;
            }
        }

        public Server pickOtherServer()
        {
            int temporary = counter % listofservers.Count;
            counter++;
            return listofservers[temporary];
        }

    }

    class BalancingTest : Balancer
    {
        private int counter;

        public BalancingTest() : base()
        {
            //nothing
        }

        public Server getServer(string ip)
        {
            //return pickOtherServer();
            Server server = checkClient(ip);

            if (server != null)
            {
                return server;
            }
            else
            {
                server = pickOtherServer();
                clientServerBinding(ip, server);
                return server;
            }
        }

        public Server pickOtherServer()
        {
            return listofservers[1];
        }
    }
}
