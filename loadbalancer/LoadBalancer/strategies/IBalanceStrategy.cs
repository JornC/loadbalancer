using System.Net.Sockets;

namespace LoadBalancer
{
    internal interface IBalanceStrategy
    {
        int determineServer(Socket client);

        void updateBalanceData(int count);
    }
}