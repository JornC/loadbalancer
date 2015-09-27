using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace FakeServer {
    class FakeServer {
        private Socket s;
        private int id;
        private int port;

        private bool running;

        private FakeServer(int port, int id) {
            this.id = id;
            this.port = port;

            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(new IPEndPoint(IPAddress.Any, port));
            s.Listen(5);
        }

        public void Listen() {
            try {
                Console.WriteLine("Server " + id + " started successfully..");
                running = true;

                while (true) {
                    Socket client = s.Accept();

                    client.Receive(new byte[1024]);
                    client.Send(System.Text.Encoding.UTF8.GetBytes("This is server " + id + "."));
                    client.Close();
                }
            } catch (Exception e) {
                running = false;
            }
        }

        public void Reboot() {
            if (running) {
                Console.WriteLine("Server " + id + " is already running..");
                return;
            }

            Console.WriteLine("Server " + id + " is rebooting.");

            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(new IPEndPoint(IPAddress.Any, port));
            s.Listen(5);

            Thread t = new Thread(Listen);
            t.Start();
        }

        public void Crash() {
            if (!running) {
                Console.WriteLine("Server " + id + " is already offline..");
                return;
            }

            Console.WriteLine("Simulating crash for server " + id);

            s.Close();
        }

        public static FakeServer StartServert(int port, int id) {
            FakeServer serv = new FakeServer(port, id);
            Thread t = new Thread(serv.Listen);
            t.Start();

            return serv;
        }
    }
}
