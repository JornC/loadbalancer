using System.Net.Sockets;

namespace LoadBalancer
{
    internal interface IServerPickert
    {
        int determineServer(Socket client);

        void updateBalanceData(int count);
    }
}