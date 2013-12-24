using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace Couchbase
{
    public class Node : INode
    {
        private readonly IPEndPoint _endPoint;
        private readonly IConnectionPool _connectionPool;
        private readonly List<IBucket> _buckets = new List<IBucket>();
        private readonly IOStrategy _ioStrategy;
        private bool _disposed;

        public Node(string server)
        {
            _endPoint = GetEndPoint(server);
        }

        public Node(string server, IConnectionPool connectionPool, IOStrategy ioStrategy) 
            :this(server)
        {
            _connectionPool = connectionPool;
            _connectionPool.EndPoint = EndPoint;
            _connectionPool.Initialize();
            _ioStrategy = ioStrategy;
        }

        public Node(string server, IConnectionPool connectionPool, List<IBucket> buckets, IOStrategy ioStrategy) 
            : this(server, connectionPool, ioStrategy)
        {
            _buckets = buckets;
        }

        public static IPEndPoint GetEndPoint(string server)
        {
            const int maxSplits = 2;
            var address = server.Split(':');
            if (address.Count() != maxSplits)
            {
                throw new ArgumentException("server");
            }
            IPAddress ipAddress;
            if (!IPAddress.TryParse(address[0], out ipAddress))
            {
                throw new ArgumentException("ipAddress");
            }
            int port;
            if (!int.TryParse(address[1], out port))
            {
                throw new ArgumentException("port");
            }
            return new IPEndPoint(ipAddress, port);
        }

        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
        }

        public IOperationResult<T> Send<T>(IOperation<T> operation)
        {
            return _ioStrategy.Execute(operation);
        }

        public IConnectionPool ConnectionPool
        {
            get { return _connectionPool; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed && _connectionPool != null)
            {
                _connectionPool.Dispose();
            }
            _disposed = true;
        }

        ~Node()
        {
            Dispose(false);
        }

        public List<IBucket> Buckets
        {
            get { return _buckets; }
        }
    }
}
