using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadbalancer
{
    class ServerInformation
    {
        public string name;
        public string ipaddress;
        public string port;

        public ServerInformation(string information)
        {
            string[] lines = information.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                switch (line)
                {
                    case "name":
                        name = lines[i + 1];
                        break;
                    case "ipaddress":
                        ipaddress = lines[i + 1];
                        break;
                    case "port":
                        port = lines[i + 1];
                        break;
                }
            }
        }
    }
}
