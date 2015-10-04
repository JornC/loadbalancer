using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer.strategies
{
    abstract class SimpleDefaultStrategy : IBalanceStrategy
    {
        private ICollection<int> serverKeys;

        public int Count
        {
            get
            {
                return serverKeys.Count;
            }
        }

        protected int getKeyFromIndex(int idx)
        {
            return serverKeys.Skip(idx).FirstOrDefault();
        }

        public virtual void updateBalanceData(ICollection<int> serverKeys)
        {
            this.serverKeys = serverKeys;
        }

        public bool Exists(int key)
        {
            return serverKeys.Contains(key);
        }

        public abstract int determineServer(IInputStreamReadWriter client, out IInputStreamReadWriter proxy);

        public abstract int determineServer(IInputStreamReadWriter client);

        public virtual IInputStreamReadWriter getResponseWrapper(int servNum)
        {
            return new SocketInputStreamReadWriter();
        }
    }
}
