using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Loadbalancer
{
    public class Server
    {
        public string name { get; set; }
        public string ipaddress { get; set; }
        public int port { get; set; }
        public DateTime lasttimeonline;
        public IPEndPoint ipendpointaddress;

        public Server(string _name, string _ipaddress, string _port)
        {
            name = _name;
            ipaddress = _ipaddress;
            port = Convert.ToInt32(_port);
            ipendpointaddress = createEndPoint();
        }

        private IPEndPoint createEndPoint()
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipaddress, out ip))
            {
                throw new FormatException("INVALID IP-ADDRESS!");
            }
            return new IPEndPoint(ip, this.port);
        }
    }
}
