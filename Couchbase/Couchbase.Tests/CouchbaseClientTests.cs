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
    public class CouchbaseClientTests
    {
        private IClient _client;
        private const string DefaultPath = @"Data\\Configs\\cluster-map.json";

        [SetUp]
        public void SetUp()
        {
            var defaultConfig = new CouchbaseClientConfig(DefaultPath);
            _client = new CouchbaseClient(defaultConfig);
        }

        [Test]
        public void Test_Set()
        {
            var key = "foopookeypoop";
            var value = "some value";
            var result = _client.Set(key, value);
            Assert.IsNotNull(result);
            Assert.IsTrue(result==null);
        }
    }
}
