using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FakeServer {
    class Program {
        static void Main(string[] args) {
            List<FakeServer> servers = new List<FakeServer>();

            servers.Add(FakeServer.StartServert(8081, 1));
            servers.Add(FakeServer.StartServert(8082, 2));
            servers.Add(FakeServer.StartServert(8083, 3));
            servers.Add(FakeServer.StartServert(8084, 4));
            servers.Add(FakeServer.StartServert(8085, 5));
            servers.Add(FakeServer.StartServert(8086, 6));
            servers.Add(FakeServer.StartServert(8087, 7));
            servers.Add(FakeServer.StartServert(8088, 8));

            Thread.Sleep(50);

            while (true) {
                Console.WriteLine();
                Console.Write("Input: ");
                string input = Console.ReadLine();

                input = input.ToLower();

                if (input.Length < 5) {
                    Console.WriteLine("Invalid command.");
                } else if (input.IndexOf("crash", 0, 5) == 0) {
                    int server = Int32.Parse(input.Substring(6, 1));
                    server = (server < 1) ? (0) : ((server > servers.Count) ? (servers.Count - 1) : (server - 1));
                    servers.ElementAt(server).Crash();
                } else if (input.IndexOf("reboot", 0, 6) == 0) {
                    int server = Int32.Parse(input.Substring(7, 1));
                    server = (server < 1) ? (1) : ((server > servers.Count) ? (servers.Count) : (server - 1));
                    servers.ElementAt(server).Reboot();
                } else {
                    Console.WriteLine("Invalid command.");
                }

                Thread.Sleep(50);
            }
        }
    }
}
