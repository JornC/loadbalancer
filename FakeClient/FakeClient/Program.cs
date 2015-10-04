using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FakeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FakeClient client = new FakeClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

            Console.Title = "Test client.";

            while(true)
            {
                Console.WriteLine("Input:");

                String input = Console.ReadLine();

                if(input.Equals("test"))
                {
                    client.doTestRun(6);
                    continue;
                }

                if(input.StartsWith("test"))
                {
                    client.doTestRun(Int32.Parse(input.Split(' ').Skip(1).First()));
                    continue;
                }

                switch(input)
                {
                    case "large":
                        client.doTestLarge();
                        break;
                    case "clear":
                        client.clearSession();
                        break;
                    default:
                        break;

                }
            }
        }
    }
}
