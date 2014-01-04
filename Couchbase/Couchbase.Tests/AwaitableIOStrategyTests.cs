using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Configuration;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;
using Couchbase.Configuration.Provider.FileSystem;
using Couchbase.IO;
using Couchbase.IO.Operations;
using Couchbase.IO.Strategies;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class AwaitableIOStrategyTests
    {
        private const string DefaultPath = @"Data\\Configs\\localhost-clustermap-default.json";
        private const string Server = "127.0.0.1:11210";
        private readonly ICouchbaseClientConfig _defaultClientConfig = new CouchbaseClientConfig(DefaultPath);
        private IConfigProvider _provider;
        private IConfigInfo _configInfo;
        private IVBucket _vBucket;
        private IClusterState _state;

        private IConnectionPool _connectionPool;
        private IConnectionPoolConfig _config;
        private AwaitableIOStrategy _ioStrategy;

        protected const string Key = "Hello";
        protected const string Value = "World";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _config = new ConnectionPoolConfig(10, 5, 1000, 1000, 1000, 1000);

            var factory = ConnectionFactory.GetDefault();
            _connectionPool = new DefaultConnectionPool(_config, Node.GetEndPoint(Server), factory);
            _connectionPool.Initialize();

            _provider = new FileSystemConfigProvider(_defaultClientConfig, ConnectionFactory.GetDefault());
            _configInfo = _provider.GetCached();

            _ioStrategy = new AwaitableIOStrategy(_connectionPool, null);
            _state = _state = new ClusterState(_defaultClientConfig);
        }

        [Test]
        public void TestSend()
        {
            _vBucket = _state.GetVBucket(Key);
            var operation = new GetOperation<string>(Key, _vBucket);
            var result = _ioStrategy.Execute(operation);
          
            Console.WriteLine(result.Value);
        }

        [Test]
        public async void TestSendAsync()
        {
            Console.WriteLine("Start thread: {0}", Thread.CurrentThread.ManagedThreadId);
            _vBucket = _state.GetVBucket(Key);
            var operation = new GetOperation<string>(Key, _vBucket);
            var result = await _ioStrategy.ExecuteAsync(operation);
            Assert.IsNotNullOrEmpty(result.Value);
            Console.WriteLine("Result for key '{0}' is '{1}'", operation.Key, result.Value);
        }

        [Test]
        public async void TestSendAsync_100()
        {
            Console.WriteLine("Start thread: {0}", Thread.CurrentThread.ManagedThreadId);

            for (var i = 0; i < 100; i++)
            {
                _vBucket = _state.GetVBucket(Key);
                var operation = new GetOperation<string>(Key, _vBucket);
                var result = await _ioStrategy.ExecuteAsync(operation);
                Assert.IsNotNullOrEmpty(result.Value);
                Console.WriteLine("Result for key '{0}' is '{1}'", operation.Key, result.Value);
            }
        }

        [Test]
        public async void TestSendAsync_100_PartDuex()
        {
            Console.WriteLine("Start thread: {0}", Thread.CurrentThread.ManagedThreadId);
            for (var i = 0; i < 100; i++)
            {
                await Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine("task thread: {0}", Thread.CurrentThread.ManagedThreadId);
                    _vBucket = _state.GetVBucket(Key);
                    var operation = new GetOperation<string>(Key, _vBucket);
                    var result = await _ioStrategy.ExecuteAsync(operation);
                    Assert.IsNotNullOrEmpty(result.Value);
                    Console.WriteLine("Result for key '{0}' is '{1}'", operation.Key, result.Value);
                });
            }
        }
    }
}
