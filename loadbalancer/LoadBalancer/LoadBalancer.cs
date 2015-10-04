using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

using System.Threading;

namespace LoadBalancer {
    class LoadBalancer {
        private Dictionary<int, IPEndPoint> servers = new Dictionary<int, IPEndPoint>();

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
            checker.SetInterval(1500);
            checker.SetServers(servers);
            checker.Crash += CrashServer;
            checker.Reboot += RebootServer;

            serverPicker = new RoundRobinBalanceStrategy();

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
            servers.Add(servers.Count, ep);
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
            servers.Remove(id);
            servers.Add(id, ep);
        }

        /// <summary>
        /// Removes the server completely from the server list.
        /// </summary>
        /// <param name="ep"></param>
        public void CrashServer(IPEndPoint ep, int id) {
            servers.Remove(id);
            updateBalanceData();
        }

        /// <summary>
        /// Dumps a simple server overview to the console.
        /// </summary>
        public void DumpOverview() {
            foreach(KeyValuePair<int, IPEndPoint> entry in servers) {
                IPEndPoint value = entry.Value;
                Console.WriteLine("Server {0}: {1}:{2}", entry.Key, value.Address, value.Port);
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
                IInputStreamReadWriter client = SocketInputStreamReadWriter.Wrap(listener.Accept());
                
                if (!running) break;

                IInputStreamReadWriter proxy;

                string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                int servNum = serverPicker.determineServer(client, out proxy);

                if (!servers.ContainsKey(servNum))
                {
                    proxy.Close();
                    Console.WriteLine("Server was not found (list empty?): {0}", servNum);
                    continue;
                }

                IPEndPoint server = servers[servNum];

                Conduit.HandleRequest(proxy, server);
                Console.WriteLine("Request received, forwarding to server {0}. Destination: {1}:{2}, Origin: {3}", servNum + 1, server.Address, server.Port, ip);
            }
        }

        private void updateBalanceData() {
            serverPicker.updateBalanceData(servers.Keys);
        }
    }
}
