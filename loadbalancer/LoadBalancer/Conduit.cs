using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.CompilerServices;

namespace LoadBalancer {
    class Conduit {
        private IInputStreamReadWriter client;
        private IPEndPoint serverEndPoint;

        private Conduit(IInputStreamReadWriter client, IPEndPoint serverEndPoint) {
            this.client = client;
            this.serverEndPoint = serverEndPoint;
        }

        private void HandleRequest() {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try {
                server.Connect(serverEndPoint);

                IInputStreamReadWriter serverWrapper = SocketInputStreamReadWriter.Wrap(server);

                ForwardResponse(client, serverWrapper);
                ForwardResponse(serverWrapper, client);
            } catch (Exception e) {
                Console.WriteLine(e);
                Console.WriteLine("Something's gone wrong, closing connection..");
                
            } finally {
                server.Close();
                client.Close();
            }
        }

        private void ForwardResponse(IInputStreamReadWriter origin, IInputStreamReadWriter destination) {
            byte[] ba = new byte[1024];
            int length;
            while (true) {
                Console.Write("Receiving in conduit - {0} .... ", origin.GetType().Name);
                length = origin.Receive(ba);
                Console.WriteLine(" ... done.");
                destination.Send(ba, length, SocketFlags.None);
                Console.WriteLine("Done 2.");

                if (length < ba.Length) break;
            }
        }

        /// <summary>
        /// Handles a request asynchronically, distributes it to one of the servers
        /// </summary>
        /// <param name="client">Socket connection with a client</param>
        /// <param name="servers">List of IPEndPoints of servers</param>
        public static void HandleRequest(IInputStreamReadWriter client, IPEndPoint serverEndPoint) {
            Conduit p = new Conduit(client, serverEndPoint);
            Thread t = new Thread(p.HandleRequest);
            t.Start();
        }
    }
}
