using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

using System.Threading;

namespace LoadBalancer {
    class LoadBalancer {
        private List<IPEndPoint> servers = new List<IPEndPoint>();

        private CrashChecker checker;

        private bool running = false;

        private Socket listener;
        private IBalanceStrategy serverPicker;

        /// <summary>
        /// Initializes a load balancer.
        /// </summary>
        /// <param name="port">Port number the load balancer will run on</param>
        public LoadBalancer(int port) {
            checker = new CrashChecker();
            checker.SetInterval(2500);
            checker.SetServers(servers);
            checker.Crash += CrashServer;
            checker.Reboot += RebootServer;

            serverPicker = new RoundRobinServerPicker();

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 8080));
            listener.Listen(5);
        }

        /// <summary>
        /// Adds a server to the server list the load balancer will distribute requests to.
        /// </summary>
        /// <remarks>
        /// Usage while running is ill-advised.
        /// </remarks>
        /// <param name="ip">IP Address of the server</param>
        /// <param name="port">Port number the server is running on</param>
        public void AddServer(IPEndPoint ep) {
            servers.Add(ep);
            checker.SetServers(servers);
            updateBalanceData();
        }

        internal void setServerPicker(IBalanceStrategy serverPicker)
        {
            this.serverPicker = serverPicker;
            updateBalanceData();
        }

        /// <summary>
        /// Reboots a server, restores it's IPEndPoint in the server list.
        /// </summary>
        /// <param name="ep">Server location</param>
        /// <param name="id">Server id</param>
        public void RebootServer(IPEndPoint ep, int id) {
            servers.RemoveAt(id);
            servers.Insert(id, ep);
        }

        /// <summary>
        /// Removes the server completely from the server list.
        /// </summary>
        /// <param name="ep"></param>
        public void CrashServer(IPEndPoint ep, int id) {
            bool correctLast = false;
            if (ep == servers.Last()) {
                correctLast = true;
            }

            for(int i = servers.Count - 1; i >= 0; i--) {
                if(servers.ElementAt(i) == ep) {
                    servers.RemoveAt(i);
                    servers.Insert(i, servers.ElementAt((i < servers.Count) ? (i) : (0)));
                }
            }

            if (correctLast) {
                servers.RemoveAt(servers.Count - 1);
                servers.Insert(servers.Count, servers.ElementAt(0));
            }
        }

        /// <summary>
        /// Dumps a simple server overview to the console.
        /// </summary>
        public void DumpOverview() {
            for(int i = 0; i < servers.Count; i++) {
                IPEndPoint ep = servers.ElementAt(i);
                Console.WriteLine("Server "+(i+1)+": "+ep.Address + ":" + ep.Port);
            }
        }

        /// <summary>
        /// Stops the load balancer
        /// </summary>
        public void StopBalancing() {
            running = false;
        }

        /// <summary>
        /// Starts the load balancer asynchronically.
        /// </summary>
        public void StartBalancingAsync() {
            Thread t = new Thread(StartBalancing);
            t.Start();
        }

        /// <summary>
        /// Starts the load balancer, (infinite) blocking call.
        /// </summary>
        public void StartBalancing() {
            if (running) return;
            running = true;

            checker.Start();

            Console.WriteLine("Listening for clients..");
            Console.WriteLine();

            while (running) {
                Socket client = listener.Accept();

                if (!running) break;

                int servNum = serverPicker.determineServer(client);
                IPEndPoint server = servers.ElementAt(servNum);
                string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

                Conduit.HandleRequest(client, server);
                Console.WriteLine("Request received, forwarding to server {0}. Destination: {1}:{2}, Origin: {3}", servNum + 1, server.Address, server.Port, ip);
            }
        }

        private void updateBalanceData() {
            serverPicker.updateBalanceData(servers.Count);
        }
    }
}
