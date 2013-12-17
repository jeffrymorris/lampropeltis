using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;
using Couchbase.Configuration.Provider.FileSystem;
using Couchbase.IO;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class FileSystemConfigProviderTests
    {
        private const string DefaultPath = @"Data\\Configs\\cluster-map.json";
        private readonly ICouchbaseClientConfig _defaultClientConfig = new CouchbaseClientConfig(DefaultPath);
        private IConfigProvider _provider;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _provider = new FileSystemConfigProvider( _defaultClientConfig, ConnectionFactory.GetDefault());
        }

        [Test]
        public void Test_GetCached()
        {
            var configInfo = _provider.GetCached();
            Assert.IsNotNull(configInfo);
            Assert.AreEqual(2, configInfo.GetServers().Count);
        }
    }
}
