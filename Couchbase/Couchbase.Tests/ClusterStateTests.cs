using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class ClusterStateTests
    {
        private IClusterState _state;
        private const string DefaultPath = @"Data\\Configs\\cluster-map.json";
        private readonly ICouchbaseClientConfig _defaultClientConfig = new CouchbaseClientConfig(DefaultPath);

        [SetUp]
        public void SetUp()
        {
            _state = new ClusterState(_defaultClientConfig);
        }

        [Test]
        public void Test_Ctor()
        {
           Assert.IsNotNull(_state.Config);
           Assert.IsNotEmpty(_state.ConfigProviders);
           Assert.AreEqual(3, _state.ConfigProviders.Count);
        }

        [Test]
        public void Test_GetServerForKey()
        {
            const string key = "somefookey";
            var server = _state.GetServerForKey(key);
            Assert.IsNotNull(server);
        }

        [Test]
        public void When_Server_Is_Retrieved_ConnectionPool_Is_Happy()
        {
            const string key = "somefookey";
            var server = _state.GetServerForKey(key);
            Assert.IsNotNull(server.ConnectionPool.EndPoint);
            var connection = server.ConnectionPool.Acquire();
            Assert.IsTrue(connection.Handle.Connected);
        }
    }
}
