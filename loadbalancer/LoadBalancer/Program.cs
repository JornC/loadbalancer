using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoadBalancer {
    class Program {
        static void Main(string[] args) {
            LoadBalancer lb = new LoadBalancer(8080);

            Console.Title = "Load Balancer";

            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8082));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8083));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8084));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8085));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8086));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8087));
            lb.AddServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8088));

            Console.WriteLine("===  Server Overview ===");
            lb.DumpOverview();

            Console.WriteLine();
            Console.WriteLine("The load balancer is zwengeling aan..");

            Console.WriteLine();
            Console.WriteLine("Press any key to start.");
            Console.ReadKey(true);
            lb.StartBalancingAsync();

            while (true)
            {
                Console.WriteLine("Input:");
                String input = Console.ReadLine();

                switch (input)
                {
                    case "round robin":
                        lb.setServerPicker(new RoundRobinServerPicker());
                        Console.WriteLine("Switched to Round Robin picking..");
                        break;
                    default:
                    case "ip":
                        lb.setServerPicker(new IPServerPicker());
                        Console.WriteLine("Switched to IP based picking..");
                        break;
                }
            }
        }
    }
}
