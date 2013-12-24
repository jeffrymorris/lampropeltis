using Couchbase.Configuration.Client;
using Couchbase.IO;
using NUnit.Framework;

namespace Couchbase.Tests.Client
{
    [TestFixture]
    public class CouchbaseClientTests
    {
        private IClient _client;
        private const string DefaultPath = @"Data\\Configs\\localhost-clustermap-default.json";

        [SetUp]
        public void SetUp()
        {
            var defaultConfig = new CouchbaseClientConfig(DefaultPath);
            _client = new CouchbaseClient(defaultConfig);
        }

        [Test]
        public void Test_Set_String()
        {
            const string key = "foopookeypoop";
            const string value = "some value";
            var operationResult = _client.Set(key, value);
            Assert.IsNotNull(operationResult);
            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
        }

        [Test]
        public void Test_Set_Int32()
        {
            const string key = "foopookeypoop2";
            const int value = 101;
            var operationResult = _client.Set(key, value);
            Assert.IsNotNull(operationResult);
            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
        }

        [Test]
        public void Test_Get_Int32()
        {
            const string key = "foopookeypoop2";
            const int value = 101;
            var operationResult = _client.Get<int>(key);
            Assert.IsNotNull(operationResult);
            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
            Assert.AreEqual(operationResult.Value, value);
        }

        [Test]
        public void Test_Get_String()
        {
            const string key = "foopookeypoop";
            const string value = "some value";
            var operationResult = _client.Get<string>(key);
            Assert.IsNotNull(operationResult);
            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
            Assert.AreEqual(operationResult.Value, value);
        }
    }
}
