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
        public int determineServer(Socket client)
        {
            throw new NotImplementedException();
        }

        public void updateBalanceData(int count)
        {
            throw new NotImplementedException();
        }
    }
}
