using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Loadbalancer
{
    public partial class HealthCheck : Window
    {
        ServerController servercontroller;
        UdpClient udplistener;
        bool keepchecking = true;
        int port = 1111;

        public HealthCheck()
        {
            InitializeComponent();
            servercontroller = ServerController.Instance;
            ThreadPool.QueueUserWorkItem(new WaitCallback(getHealthReport));
            ThreadPool.QueueUserWorkItem(new WaitCallback(checkServers));
        }

        private void getHealthReport(object obj)
        {
            try
            {
                IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, this.port);
                this.udplistener = new UdpClient(this.port);
                while (keepchecking)
                {
                    byte[] bytes = udplistener.Receive(ref ipendpoint);
                    string servermessage = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Console.WriteLine("Server {0}:\n {1}\n", ipendpoint.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                    servercontroller.checkServers(servermessage, ipendpoint);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SOCKETEXCEPTION: {0}", e);
            }
        }

        private void checkServers(object obj)
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke(
                    (Action)delegate()
                    {
                        List<Server> listofservers = servercontroller.listofservers;
                        for (int i = 0; i < listofservers.Count(); i++)
                        {
                            Server server = listofservers[i];
                            DateTime lasttimeonline = server.lasttimeonline;
                            DateTime currentlyonline = lasttimeonline.AddSeconds(5);

                            if (currentlyonline.CompareTo(DateTime.Now) < 0)
                            {
                                servercontroller.removeServer(server);
                            }
                        }
                        servercontroller.numberofservers = listofservers.Count();
                        setHealthNotifier();
                    }
                );
                Thread.Sleep(4000);
            }
        }

        private void updateHealthNotifier(object obj)
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke(
                    (Action)delegate()
                    {
                        setHealthNotifier();
                    }
                );
                Thread.Sleep(1000);
            }
        }

        private void setHealthNotifier()
        {
            healthnotifier.Items.Clear();
            List<Server> listofservers= servercontroller.listofservers;
            for (int i = 0; i < listofservers.Count(); i++)
            {
                Server server = listofservers[i];
                healthnotifier.Items.Add(server.name);
            }
        }

    }
}
