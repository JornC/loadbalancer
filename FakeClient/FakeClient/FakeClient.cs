using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeClient
{
    class FakeClient
    {
        private IPEndPoint server;

        private string sessionID;

        public FakeClient(IPEndPoint server)
        {
            this.server = server;
        }

        public void doTestRun(int num)
        {
            for (int i = 0; i < num; i++)
            {
                send("test");
            }
        }

        public void doTestLarge()
        {
            send("/large", "hoi");
        }

        private void send(String things)
        {
            send("/", things);
        }

        private void send(String path, String things)
        {
            if (sessionID != null)
            {
                things = String.Format("GET {0} HTTP/1.1\r\nCookie:JSESSIONID={1};\r\n\r\n", path, sessionID) + things;
            }

            byte[] ba = new byte[1024];

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Connect(server);
            socket.Send(Encoding.ASCII.GetBytes(things));
            int length = socket.Receive(ba);

            string content = Encoding.ASCII.GetString(ba, 0, length);

            int idx;
            if ((idx = content.IndexOf("JSESSIONID")) != -1)
            {
                Console.WriteLine("Saving session ID / cookie.");

                sessionID = content.Substring(idx, content.IndexOf(';', idx)).Split('=').Skip(1).First();
            }

            Console.WriteLine("Request: {0}", path);
            Console.WriteLine("Response: {0}", content);
            Console.WriteLine();

            int cnt = 0;
            while (length == ba.Length)
            {
                if(cnt++ == 0)
                {
                    Console.WriteLine("Receiving many..");
                }
                length = socket.Receive(ba);
            }

            Console.WriteLine("Closing.. {0} @ {1}", length, cnt);


            socket.Close();
        }

        internal void clearSession()
        {
            sessionID = null;
            Console.WriteLine("Session cleared.");
        }
    }
}
