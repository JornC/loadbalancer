using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FakeServer {
    class Program {
        static void Main(string[] args) {
            List<FakeServer> servers = new List<FakeServer>();

            servers.Add(FakeServer.StartServer(8081, 1));
            servers.Add(FakeServer.StartServer(8082, 2));
            servers.Add(FakeServer.StartServer(8083, 3));
            servers.Add(FakeServer.StartServer(8084, 4));
            servers.Add(FakeServer.StartServer(8085, 5));
            servers.Add(FakeServer.StartServer(8086, 6));
            servers.Add(FakeServer.StartServer(8087, 7));
            servers.Add(FakeServer.StartServer(8088, 8));

            Thread.Sleep(50);

            while (true) {
                Console.WriteLine();
                Console.Write("Input: ");
                string input = Console.ReadLine();

                input = input.ToLower();

                if (input.StartsWith("crash")) {
                    int serverId = Int32.Parse(input.Substring(6, 1));
                    serverId = (serverId < 1) ? (0) : ((serverId > servers.Count) ? (servers.Count - 1) : (serverId - 1));
                    servers.ElementAt(serverId).Crash();
                } else if (input.StartsWith("reboot")) {
                    int serverId = Int32.Parse(input.Substring(7, 1));
                    serverId = (serverId < 1) ? (0) : ((serverId > servers.Count) ? (servers.Count) : (serverId - 1));
                    servers.ElementAt(serverId).Reboot();
                } else {
                    Console.WriteLine("Invalid command.");
                }

                Thread.Sleep(50);
            }
        }
    }
}
