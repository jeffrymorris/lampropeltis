﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Couchbase.Configuration.Server;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace Couchbase
{
    public class Server : IServer
    {
        private readonly IPEndPoint _endPoint;
        private readonly IConnectionPool _connectionPool;
        private readonly List<IBucket> _buckets = new List<IBucket>();
        private bool _disposed;

        public Server(string server)
        {
            _endPoint = GetEndPoint(server);
        }

        public Server(string server, IConnectionPool connectionPool) 
            :this(server)
        {
            _connectionPool = connectionPool;
            _connectionPool.EndPoint = EndPoint;
            _connectionPool.Initialize();
        }

        public Server(string server, IConnectionPool connectionPool, List<IBucket> buckets)
            : this(server, connectionPool)
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

        public IOperationResult Send(IOperation operation)
        {
            throw new NotImplementedException();
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

        ~Server()
        {
            Dispose(false);
        }

        public List<IBucket> Buckets
        {
            get { return _buckets; }
        }
    }
}
