using LoadBalancer.sock;
using LoadBalancer.strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class SessionPersistentBalanceStrategy : SimpleDefaultStrategy
    {
        private const int BUFFER_SIZE = 1024;

        Dictionary<string, int> sessionMap = new Dictionary<string, int>();

        private IBalanceStrategy backup = new RoundRobinBalanceStrategy();

        public override int determineServer(IInputStreamReadWriter client)
        {
            throw new NotImplementedException();
        }

        public override int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
       { 
            proxy = new MockInputStreamReadWriter(client.getSocket());

            byte[] ba = new byte[BUFFER_SIZE];
            int length;
            string content = "";
            while (true)
            {
                length = client.Receive(ba);

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
                    Console.WriteLine("No session found - doing backup strategy.");
                    return backup.determineServer(proxy);
                } else
                {
                    Console.WriteLine("Found session with id {0}", sessionID);
                }

                if (!sessionMap.ContainsKey(sessionID))
                {
                    Console.WriteLine("Session not (yet) in cache - falling back to backup.");
                    InsertSession(sessionID, backup.determineServer(proxy));
                }

                int serverNumber = sessionMap[sessionID];

                if(!Exists(serverNumber))
                {
                    Console.WriteLine("Server does not exist (anymore)! Session:{0} serverNum:{1}", sessionID, serverNumber);
                    serverNumber = backup.determineServer(proxy);
                    InsertSession(sessionID, serverNumber);
                }

                return serverNumber;
            } else
            {
                return backup.determineServer(proxy);
            }
        }

        internal void InsertSession(string sessionID, int servNum)
        {
            sessionMap[sessionID] = servNum;
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

        public override IInputStreamReadWriter getResponseWrapper(int servNum)
        {
            return new PersistentInputStreamReadWriter(this, servNum);
        }

        public override void updateBalanceData(ICollection<int> serverKeys)
        {
            base.updateBalanceData(serverKeys);
            backup.updateBalanceData(serverKeys);
        }
    }
}
