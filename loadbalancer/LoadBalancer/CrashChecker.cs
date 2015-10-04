using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LoadBalancer {
    class CrashChecker {
        private Dictionary<int, IPEndPoint> servers;
        private List<Check> checkers;

        public delegate void ServerNotification(IPEndPoint ep, int id);
        public event ServerNotification Crash;
        public event ServerNotification Reboot;

        private int interval;
        private bool running;

        /// <summary>
        /// Sets the server list
        /// </summary>
        /// <param name="servers">List of servers</param>
        public void SetServers(Dictionary<int, IPEndPoint> servers) {
            this.servers = servers;

            if (running) {
                Restart();
            }
        }

        /// <summary>
        /// Sets the interval
        /// </summary>
        /// <param name="milliseconds">Number of milliseconds</param>
        public void SetInterval(int interval) {
            this.interval = interval;

            if (running) {
                Restart();
            }
        }

        /// <summary>
        /// Starts checking for server crashes
        /// </summary>
        public void Start() {
            if(running) return;
            running = true;

            checkers = new List<Check>();

            foreach(KeyValuePair<int, IPEndPoint> entry in servers) {
                Check cc = new Check(entry.Value, interval, entry.Key);
                cc.Crash += CrashDetected;
                cc.Reboot += RebootDetected;
                checkers.Add(cc);

                Thread t = new Thread(cc.Start);
                t.Start();
            }
        }

        /// <summary>
        /// Restarts the checking process
        /// </summary>
        public void Restart() {
            Stop();
            Start();
        }

        /// <summary>
        /// Stops checking for server crashes
        /// </summary>
        public void Stop() {
            foreach(Check c in checkers) {
                c.Stop();
            }

            running = false;
        }

        private void CrashDetected(IPEndPoint ep, int id) {
            Console.WriteLine("Server {0} just endured a crash, notifying load balancer..", id + 1);
            Crash(ep, id);
        }

        private void RebootDetected(IPEndPoint ep, int id) {
            Console.WriteLine("Server {0} magically re-entered my radar, notifying load balancer..", id + 1);
            Reboot(ep, id);
        }

        #region Inner class Check

        /// <summary>
        /// Inner class to check a single server for crashes
        /// </summary>
        class Check {
            private IPEndPoint ep;
            private int interval;
            private int id;

            private bool running;
            private bool serverOnline = true;

            public delegate void ServerNotification(IPEndPoint ep, int id);
            public event ServerNotification Crash;
            public event ServerNotification Reboot;

            public Check(IPEndPoint ep, int interval, int id) {
                this.id = id;
                this.ep = ep;
                this.interval = interval;
            }

            /// <summary>
            /// Starts the crashcheck
            /// </summary>
            /// <remarks>
            /// Should be thrown into another thread
            /// </remarks>
            public void Start() {
                if (running) return;
                running = true;

                while (running) {
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    try {
                        s.Connect(ep);
                    } catch (Exception e) {
                        if (!running) break;

                        if (serverOnline) {
                            serverOnline = false;
                            Crash(ep, id);
                        }
                    }

                    if (!serverOnline && s.Connected) {
                        serverOnline = true;
                        Reboot(ep, id);
                    }
                    s.Close();

                    Thread.Sleep(interval);
                }
            }

            /// <summary>
            /// Stops the crash check
            /// </summary>
            public void Stop() {
                running = false;
            }
        }

        #endregion
    }
}
