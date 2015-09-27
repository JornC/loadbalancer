using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Loadbalancer
{
    class Proxy
    {
        public bool isalive { get; set; }
        ServerController servercontroller;
        MainWindow window;
        private int port = 8080;
        private TcpListener tcplistener;
        BalancingTest balancingtest;

        public void start(MainWindow mainwindow)
        {
            servercontroller = ServerController.Instance;
            balancingtest = new BalancingTest();
            this.window = mainwindow;
            port = getPortNumber();
            establishConnection();
        }

        private int getPortNumber()
        {
            return convertPort(window.portinput.Text);
        }

        private int convertPort(string porttext)
        {
            int port;
            if (porttext.Length > 0)
            {
                port = Convert.ToInt32(porttext);
            }
            else
            {
                port = 0;
            }
            return port;
        }

        private void establishConnection()
        {
            if (isPortValid(port))
            {
                try
                {
                    tcplistener = new TcpListener(IPAddress.Any, port);
                    tcplistener.Start();
                    window.updateNotifier("TcpListener created");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(startAccept));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private bool isPortValid(int port)
        {
            if (port > 0)
            {
                return true;
            }
            else
            {
                window.updateNotifier("Unable to start the server");
                showWarningMessage("Incorrect portnumber");
                return false;
            }
        }

        private void startAccept(object obj)
        {
            int id = 0;
            while (this.isalive)
            {
                try
                {
                    this.window.updateNotifier("Listening");
                    TcpClient client = tcplistener.AcceptTcpClient();
                    int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
                    string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    this.window.updateNotifier("Connected to: " + port);
                    Server server = balancingtest.getServer(ip);

                    if (server != null)
                    {
                        ConnectionHandler handler = new ConnectionHandler(client, id, window, server);
                        id++;
                    }
                    else
                    {
                        window.updateNotifier("There is not a single server running");
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Er is een error, namelijk: {0}", e);
                }
            }
        }

        private MessageBoxResult showWarningMessage(string message)
        {
            return MessageBox.Show(message, "Warning");
        }

        public void stopTcpListener()
        {
            if (tcplistener != null)
            {
                tcplistener.Stop();
                window.updateNotifier("Server has been terminated.");
            }
        }
    }
}
