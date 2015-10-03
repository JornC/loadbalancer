using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class BalanceStrategyFactory
    {
        internal static IBalanceStrategy getStrategy(string input)
        {
            switch (input)
            {
                case "ip":
                    return new IPBalanceStrategy();
                case "session":
                case "session persistent":
                    return new SessionPersistentBalanceStrategy();
                case "asession":
                case "agressive session persistent":
                    return new AggressiveSessionPersistentBalanceStrategy();
                default:
                case "round":
                case "round robin":
                    return new RoundRobinBalanceStrategy();
            }
        }
    }
}
