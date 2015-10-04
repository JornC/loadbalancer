using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class AggressiveSessionPersistentBalanceStrategy : IBalanceStrategy
    {
        public int determineServer(IInputStreamReadWriter client)
        {
            throw new NotImplementedException();
        }

        public int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy)
        {
            throw new NotImplementedException();
        }

        public void updateBalanceData(ICollection<int> count)
        {
            throw new NotImplementedException();
        }
    }
}
