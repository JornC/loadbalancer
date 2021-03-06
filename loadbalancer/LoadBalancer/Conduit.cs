﻿using System;
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
        private IInputStreamReadWriter wrapper;

        private Conduit(IInputStreamReadWriter client, IPEndPoint serverEndPoint, IInputStreamReadWriter wrapper) {
            this.client = client;
            this.serverEndPoint = serverEndPoint;
            this.wrapper = wrapper;
        }

        private void HandleRequest() {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try {
                server.Connect(serverEndPoint);

                wrapper.Wrap(server);

                ForwardResponse(client, wrapper);
                ForwardResponse(wrapper, client);
            } catch (Exception e) {
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
                length = origin.Receive(ba);
                destination.Send(ba, length, SocketFlags.None);
                
                if (length < ba.Length) break;
            }
        }

        /// <summary>
        /// Handles a request asynchronically, distributes it to one of the servers
        /// </summary>
        /// <param name="client">Socket connection with a client</param>
        /// <param name="servers">List of IPEndPoints of servers</param>
        public static void HandleRequest(IInputStreamReadWriter client, IPEndPoint serverEndPoint, IInputStreamReadWriter wrapper) {
            Conduit p = new Conduit(client, serverEndPoint, wrapper);
            Thread t = new Thread(p.HandleRequest);
            t.Start();
        }
    }
}
