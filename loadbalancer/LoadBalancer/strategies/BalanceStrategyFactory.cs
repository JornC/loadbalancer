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
                    return new IPServerPicker();
                case "session":
                case "session persistent":
                    return new SessionPersistentBalanceStrategy();
                default:
                case "round":
                case "round robin":
                    return new RoundRobinServerPicker();
            }
        }
    }
}
