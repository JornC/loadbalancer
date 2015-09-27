using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Loadbalancer
{
    //Using the sealed keyword, so no other class can inherit from this class.
    public sealed class ServerController
    {
        public List<Server> listofservers { get; set; }
        public int numberofservers;
        public MainWindow window;

        //Only one instance of the ServerController is allowed, so the singleton pattern is used.
        private static readonly ServerController instance = new ServerController();

        //Hidden constructor
        private ServerController()
        {
            listofservers = new List<Server>();
        }

        //The instantiation does not occur until the class is first referenced by a call to the Instance property.
        public static ServerController Instance
        {
            get
            {
                return instance;
            }
        }

        public void addServer(Server server)
        {
            listofservers.Add(server);
        }

        public void removeServer(Server server)
        {
            listofservers.Remove(server);
        }

        public void checkServers(string servermessage, IPEndPoint ip)
        {
            ServerInformation information = new ServerInformation(servermessage);
            Server server = listofservers.Find(item => item.name == information.name);

            if (server != null) //server currently exists
            {
                server.lasttimeonline = DateTime.Now;
            }
            else //server not exists
            {
                Server serverx = new Server(information.name, information.ipaddress, information.port);
                serverx.lasttimeonline = DateTime.Now;
                this.addServer(serverx);
            }
        }
    }
}