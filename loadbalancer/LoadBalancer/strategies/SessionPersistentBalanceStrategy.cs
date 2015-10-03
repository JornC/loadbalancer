using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class SessionPersistentBalanceStrategy : IBalanceStrategy
    {
        private const int BUFFER_SIZE = 1024;

        Dictionary<string, int> sessionMap = new Dictionary<string, int>();

        private IBalanceStrategy backup = new RoundRobinBalanceStrategy();
        private int servNum;

        public int determineServer(IInputStreamReadWriter client)
        {
            throw new NotImplementedException();
        }

        public int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
       { 
            proxy = new MockInputStreamReadWriter(client.getSocket());

            byte[] ba = new byte[BUFFER_SIZE];
            int length;
            string content = "";
            while (true)
            {
                Console.Write("Receiving (balancer) .. " + client.GetType().Name);
                length = client.Receive(ba);
                Console.WriteLine(" .. Done.");

                if(length == 0)
                {
                    break;
                }

                proxy.Feed(ba, length);

                content += ASCIIEncoding.ASCII.GetString(ba, 0, length);

                if (length < ba.Length) break;
            }

            if(content.Equals(""))
            {
                Console.WriteLine("Bitchbitch.");
                return 0;
            }

            String header = content.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries).First();
            String[] headers = header.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            String cookies = null;
            foreach(string headerLine in headers)
            {
                if(headerLine.StartsWith("Cookie"))
                {
                    cookies = headerLine.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }

            if(cookies != null)
            {
                String sessionID = getSessionID(cookies);

                if(sessionID == null)
                {
                    return backup.determineServer(proxy);
                }

                if (!sessionMap.ContainsKey(sessionID))
                {
                    sessionMap.Add(sessionID, backup.determineServer(proxy));
                }

                Console.WriteLine("Session ID - {0}", sessionID);
                int serverNumber;
                sessionMap.TryGetValue(sessionID, out serverNumber);
                return serverNumber;
            } else
            {
                return backup.determineServer(proxy);
            }
        }

        private string getSessionID(string cookies)
        {
            string[] cookieSplit = cookies.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string cookie in cookieSplit)
            {
                if(cookie.Trim().StartsWith("JSESSIONID"))
                {
                    return cookie.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }

            return null;
        }

        public void updateBalanceData(int count)
        {
            backup.updateBalanceData(count);
            servNum = count;
        }
    }
}
