using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsyncEventArgsExample
{
    public class SocketAsyncEventArgsPool
    {
        private readonly ConcurrentStack<SocketAsyncEventArgs> _pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            _pool = new ConcurrentStack<SocketAsyncEventArgs>(new Stack<SocketAsyncEventArgs>(capacity));
        }

        public void Push(SocketAsyncEventArgs item)
        {
            _pool.Push(item);
        }

        public SocketAsyncEventArgs Pop()
        {
            SocketAsyncEventArgs item;
            if (!_pool.TryPop(out item))
            {
                throw new InvalidOperationException();
            }
            return item;
        }

        public int Count()
        {
            return _pool.Count;
        }
    }
}
