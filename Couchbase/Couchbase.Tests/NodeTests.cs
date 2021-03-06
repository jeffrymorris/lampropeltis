﻿using System.Collections.Generic;
using System.Net;
using Couchbase.Configuration.Client;
using Couchbase.IO;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class NodeTests
    {
        private INode _server;
        const string ServerAddress = "127.0.0.1:11210";
        private const string DefaultPath = @"Data\\Configs\\localhost-clustermap-default.json";
        private readonly ICouchbaseClientConfig _config = new CouchbaseClientConfig(DefaultPath);
        private readonly List<IBucket> _buckets = new List<IBucket> {new Bucket("default", "", "")};
        private IConnectionPool _connectionPool;

        [SetUp]
        public void SetUp()
        {
            var factory = ConnectionFactory.GetDefault();
            _connectionPool = new DefaultConnectionPool(_config.ConnectionPoolConfiguration, factory);
            _server = new Node(ServerAddress, _connectionPool, _buckets, null);
        }

        [Test]
        public void Test_EndPoint()
        {
            IPAddress ipAddress;
            IPAddress.TryParse(ServerAddress.Split(':')[0], out ipAddress);
            var ipEndpoint = new IPEndPoint(ipAddress, int.Parse(ServerAddress.Split(':')[1]));

            Assert.AreEqual(ipEndpoint, _server.EndPoint);
        }

        [Test]
        public void Test_ConnectionPool()
        {
            var connectionPool = _server.ConnectionPool;
            Assert.IsNotNull(connectionPool);
            Assert.AreEqual(_config.ConnectionPoolConfiguration.MinSize, connectionPool.Count());
        }

        [Test]
        public void Test_Dispose()
        {
            _server.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _server.Dispose();
        }
    }
}
