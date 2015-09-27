using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Loadbalancer
{
    class ConnectionHandler
    {
        private TcpClient tcpclient;
        private TcpClient tcpserver;
        private NetworkStream clientstream;
        private NetworkStream serverstream;
        private int connectionid;
        MainWindow window;
        Server server;

        public ConnectionHandler (TcpClient client, int id, MainWindow window, Server server)
        {
            this.tcpclient = client;
            this.connectionid = id;
            this.window = window;
            this.server = server;
            Thread thread = new Thread(handleConnection);
            thread.Start();
        }

        private void handleConnection()
        {
            try
            {
                //client
                this.window.updateNotifier("Connected");
                clientstream = tcpclient.GetStream();
                string clientheader = getClientHeader(clientstream);
                clientheader = tightenString(clientheader, "Proxy-Connection: keep-alive\r\n");
                string host = getHost(clientheader);

                //server
                tcpserver = new TcpClient(server.ipaddress, server.port);
                serverstream = tcpserver.GetStream();
                //write header
                serverstream.Write(System.Text.Encoding.ASCII.GetBytes(clientheader), 0, clientheader.Length);
                //get server response
                string serverheader = getServerHeader(serverstream);
                //send header to client
                sendHeaderToClient(clientstream, serverheader);
                //determine contentlength
                int contentlength = getContentLength(serverheader);
                //send content to client
                string servercontent = getContentServer(serverstream, contentlength, clientstream);
                //close connection
                window.updateNotifier("Closing connection");
                closeConnection(tcpclient);
                closeConnection(tcpserver);
            }
            catch (Exception e)
            {
                window.updateNotifier("Attempt to close connection, but an error occured");
                closeConnection(tcpclient);
                closeConnection(tcpserver);
            }
        }

        private string getClientHeader(NetworkStream clientstream)
        {
            byte[] message = new byte[1];
            string header = "";
            int bytesread;

            while (!(header.IndexOf("\r\n\r\n") > 0))
            {
                bytesread = clientstream.Read(message, 0, 1);
                header += System.Text.ASCIIEncoding.ASCII.GetString(message, 0, 1);

                if(bytesread <= 0)
                {
                    window.updateNotifier("No bytes received");
                    return "Can't read header";
                }
            }
            return header;
        }

        private string tightenString(string header, string replacement)
        {
            string str = Regex.Replace(header, replacement, "", RegexOptions.IgnoreCase);
            return str;
        }

        private string getHost(string header)
        {
            string[] lines = header.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] fields = line.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (fields[0].Equals("Host"))
                {
                    if (fields[2] != null)
                    {
                        string port = fields[2].Trim();
                    }
                    return fields[1].Trim();
                }
            }
            return "No host found";
        }

        private string getServerHeader(NetworkStream serverstream)
        {
            byte[] message = new byte[1024];
            int i =1;
            string header = "";
            
            while(!(header.IndexOf("\r\n\r\n") > 0))
            {
                i = serverstream.Read(message, 0, 1);
                header += System.Text.ASCIIEncoding.ASCII.GetString(message, 0, 1);
            }

            return header;
        }

        private void sendHeaderToClient(NetworkStream clientstream, string serverheader)
        {
            clientstream.Write(System.Text.Encoding.ASCII.GetBytes(serverheader), 0, serverheader.Length);
        }

        private int getContentLength(string serverheader)
        {
            string[] lines = serverheader.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] fields = line.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (fields[0].Equals("Content-Length"))
                {
                    return Convert.ToInt32(fields[1]);
                }
            }
            return 0;
        }

        private string getContentServer(NetworkStream serverstream, int contentlength, NetworkStream clientstream)
        {
            Console.WriteLine("Send content to client where id = {0}", connectionid);
            
            //read the body
            string bodytext = "";

            while (contentlength > 0)
            {
                int size = Math.Min(contentlength, 512);
                byte[] body = new byte[size];
                int bytesread = serverstream.Read(body, 0, size);
                
                //check the data
                if (bytesread > 0)
                {
                    contentlength -= bytesread;
                    //write content to client
                    clientstream.Write(body, 0, bytesread);
                }
                else
                {
                    window.updateNotifier("Zero bytes received");
                    contentlength = 0;
                }
            }
            return bodytext;
        }

        private void closeConnection(TcpClient client)
        {
            if (client != null && client.Connected)
            {
                client.GetStream().Close();
                client.Close();
            }
            else
            {
                window.updateNotifier("There is no connection to close");
            }
        }
    }
}