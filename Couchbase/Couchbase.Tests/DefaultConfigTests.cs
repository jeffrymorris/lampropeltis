using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Configuration;
using Couchbase.Configuration.Server;
using Couchbase.Encryption;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class DefaultConfigTests
    {
        private DefaultConfig _defaultConfig;
        private IClusterMap _clusterMap;

        [SetUp]
        public void Setup()
        {
            using (var stream = File.Open(@"Data\\Configs\\cluster-map.json", FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                   _clusterMap = JsonConvert.DeserializeObject<ClusterMap>(reader.ReadToEnd());
                }
            }

            var vBucketServerMap = _clusterMap.VBucketServerMap;
            var servers = vBucketServerMap.
                ServerList.Select(server => new Node(server)).
                Cast<INode>().
                ToList();

            var vBuckets = new Dictionary<int, IVBucket>();
            for (var i = 0; i < vBucketServerMap.VBucketMap.Length; i++)
            {
                var primary = vBucketServerMap.VBucketMap[i][0];
                var replica = vBucketServerMap.VBucketMap[i][1];
                var vBucket = new VBucket(servers, i, primary, replica);
                vBuckets[i] = vBucket;
            }
            _defaultConfig = new DefaultConfig(new Crc32(), servers, vBuckets);
        }

        [Test]
        public void TestGetIndex()
        {
            const string key = "somekeytohash";
            var index1 =_defaultConfig.GetIndex(key);
            var index2 = _defaultConfig.GetIndex(key);

            Assert.AreEqual(index1, index2);
        }

        [Test]
        public void Test_CreationTime()
        {
            Assert.LessOrEqual(_defaultConfig.CreationTime, DateTime.Now);
        }

        [Test]
        public void Test_GetServers()
        {
            var cluster =_defaultConfig.GetServers();
            Assert.IsNotEmpty(cluster);
        }

        [Test]
        public void Test_HashToVBucket()
        {
            const string key = "somekeytohash-weeee";
            var vBucket = _defaultConfig.HashToVBucket(key);

            Assert.IsNotNull(vBucket);
            Assert.AreEqual(1, vBucket.Replicas.Count);
        }

        [Test]
        public void Test_LocatePrimary()
        {
            const string key = "somekeytohash-weeee";
            var vBucketId = _defaultConfig.GetIndex(key);

            var index = _clusterMap.VBucketServerMap.VBucketMap[vBucketId];
            var expected = _clusterMap.VBucketServerMap.ServerList[index[0]];

            var vBucket = _defaultConfig.HashToVBucket(key);
            var server = vBucket.LocatePrimary();

            Assert.IsNotNull(server);
            Assert.AreEqual(expected, server.EndPoint.ToString());
        }
    }
}
