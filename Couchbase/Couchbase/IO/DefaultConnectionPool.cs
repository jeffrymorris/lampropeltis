using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using Couchbase.Configuration.Client;

namespace Couchbase.IO
{
    public class DefaultConnectionPool : IConnectionPool
    {
        private readonly ConcurrentQueue<IConnection> _store = new ConcurrentQueue<IConnection>();
        private readonly Func<IConnectionPool, IConnection> _factory;
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly IConnectionPoolConfig _config;
        private readonly object _lock = new object();
        private int _count;
        private bool _disposed;

        public DefaultConnectionPool(IConnectionPoolConfig config, Func<IConnectionPool, IConnection> factory) 
        {
            _config = config;
            _factory = factory;
        }

        public DefaultConnectionPool(IConnectionPoolConfig config, IPEndPoint endPoint, Func<IConnectionPool, IConnection> factory)
            : this(config, factory)
        {
            EndPoint = endPoint;
        }

        public void Initialize()
        {
            do
            {
                _store.Enqueue(_factory(this));
                Interlocked.Increment(ref _count);
            }
            while(_store.Count < _config.MinSize);
        }

        public IConnection Acquire()
        {
            IConnection connection;
            
            if(_store.TryDequeue(out connection))
            {
                Console.WriteLine("Acquire existing: {0}", connection.Identity);
                return connection;
            }

            lock (_lock)
            {
                if (_count < _config.MaxSize)
                {
                    connection = _factory(this);
                    Console.WriteLine("Acquire new: {0}", connection.Identity);
                    Interlocked.Increment(ref _count);
                    return connection;
                }
            }

            _autoResetEvent.WaitOne(_config.WaitTimeout);
            Console.WriteLine("Argh, trying again");
            return Acquire();
        }

        public void Release(IConnection connection)
        {
            Console.WriteLine("Releasing: {0}", connection.Identity);
            _store.Enqueue(connection);
            _autoResetEvent.Set();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                while (_store.Count > 0)
                {
                    IConnection connection;
                    if (_store.TryDequeue(out connection))
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        ~DefaultConnectionPool()
        {
            Dispose(false);
        }

        public int Count()
        {
            return _count;
        }

        public IConnectionPoolConfig Config
        {
            get { return _config; }
        }


        public IPEndPoint EndPoint { get; set; }
    }
}
