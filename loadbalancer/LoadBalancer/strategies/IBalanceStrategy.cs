using System.Collections.Generic;
using System.Net.Sockets;

namespace LoadBalancer
{
    internal interface IBalanceStrategy
    {
        int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy);
        
        int determineServer(IInputStreamReadWriter client);

        void updateBalanceData(ICollection<int> count);

        IInputStreamReadWriter getResponseWrapper(int servNum);
    }
}