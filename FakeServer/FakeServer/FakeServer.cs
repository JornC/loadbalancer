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
        private ManualResetEvent handle = new ManualResetEvent(false);

        private bool running;

        private FakeServer(int port, int id) {
            this.id = id;
            this.port = port;

            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(new IPEndPoint(IPAddress.Any, port));
            s.Listen(100);
        }

        public void Listen() {
            try {
                Console.WriteLine("Server " + id + " started successfully..");
                running = true;

                while (running) {
                    handle.Reset();
                    s.BeginAccept(new AsyncCallback(handleConnection), s);
                    handle.WaitOne();
                }
            } catch (Exception e) {
                running = false;
            }
        }

        public void handleConnection(IAsyncResult callback)
        {
            handle.Set();

            Socket client = (Socket) callback.AsyncState;
            try {
                Socket connection = client.EndAccept(callback);

                byte[] ba = new byte[1024];
                int length = connection.Receive(ba);

                string lolz = ASCIIEncoding.ASCII.GetString(ba, 0, length);
                if (lolz.Contains("JSESSIONID"))
                {
                    connection.Send(System.Text.Encoding.UTF8.GetBytes(String.Format("This server {1}", id, id)));
                } else
                {
                    connection.Send(System.Text.Encoding.UTF8.GetBytes(String.Format("HTTP/1.1 200 OK\r\nset-cookie: JSESSIONID={0}; expires=Sat, 02 May 2016 23:38:25 GMT;\r\n\r\nThis is server {1}", id, id)));
                }

                connection.Close();
            } catch (Exception e)
            {
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

            try {
                s.Close();
            } catch (Exception e)
            {

            }
        }

        public static FakeServer StartServert(int port, int id) {
            FakeServer serv = new FakeServer(port, id);
            Thread t = new Thread(serv.Listen);
            t.Start();

            return serv;
        }
    }
}
