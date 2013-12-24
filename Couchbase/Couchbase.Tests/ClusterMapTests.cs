using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Configuration.Server;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class ClusterMapTests
    {
        private IClusterMap _clusterMap;

        [TestFixtureSetUp]
        public void TestSeserialization()
        {
            using (var stream = File.Open(@"Data\\Configs\\cluster-map.json", FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                   _clusterMap = JsonConvert.DeserializeObject<ClusterMap>(reader.ReadToEnd());
                }
            }
        }

        [Test]
        public void TestDeserialization()
        {
            Assert.IsNotNull(_clusterMap);
        }

        [Test]
        public void TestClusterMap()
        {
            const string name = "default";
            const string bucketType = "membase";
            const string authType = "sasl";
            const string saslPassword = "";
            const int proxyPort = 0;
            const bool replicaIndex = false;
            var uri = new Uri("/pools/default/buckets/default?bucket_uuid=47db989ce4d0a4a54ff8a281826499a9", UriKind.Relative);
            var streamingUri = new Uri("/pools/default/bucketsStreaming/default?bucket_uuid=47db989ce4d0a4a54ff8a281826499a9", UriKind.Relative);
            var localRandomKeyUri = new Uri("/pools/default/buckets/default/localRandomKey", UriKind.Relative);
            const string nodeLocator = "vbucket";
            const bool autoCompactionSettings = false;
            const bool fastWarmupSettings = false;
            const string uuid = "47db989ce4d0a4a54ff8a281826499a9";
            const string bucketCapabilitiesVer = "";
            const int statsCount = 3;

            Assert.AreEqual(name, _clusterMap.Name);
            Assert.AreEqual(bucketType, _clusterMap.BucketType);
            Assert.AreEqual(authType, _clusterMap.AuthType);
            Assert.AreEqual(saslPassword, _clusterMap.SaslPassword);
            Assert.AreEqual(proxyPort, _clusterMap.ProxyPort);
            Assert.AreEqual(replicaIndex, _clusterMap.ReplicaIndex);
            Assert.AreEqual(uri, _clusterMap.Uri);
            Assert.AreEqual(streamingUri, _clusterMap.StreamingUri);
            Assert.AreEqual(localRandomKeyUri, _clusterMap.LocalRandomKeyUri);
            Assert.AreEqual(nodeLocator, _clusterMap.NodeLocator);
            Assert.AreEqual(autoCompactionSettings, _clusterMap.AutoCompactionSettings);
            Assert.AreEqual(fastWarmupSettings, _clusterMap.FastWarmupSettings);
            Assert.AreEqual(uuid, _clusterMap.Uuid);
            Assert.AreEqual(bucketCapabilitiesVer, _clusterMap.BucketCapabilitiesVer);
            Assert.IsNotNull(_clusterMap.VBucketServerMap);
            Assert.AreEqual(statsCount, _clusterMap.Stats.Count);
        }

        [Test]
        public void TestNodes()
        {
            const int nodesCount = 2;
            var expected = new Configuration.Server.Node
            {
                CouchAPIBase = new Uri("http://192.168.56.102:8092/default"),
                Replication = "0",
                ClusterMembership = "active",
                Status = "warmup",
                ThisNode = true,
                Hostname = "192.168.56.102:8091",
                ClusterCompatibility = 131072,
                Version = "2.1.1-764-rel-enterprise",
                OS = "x86_64-unknown-linux-gnu"
            };
            expected.Ports.Add("proxy", 11211);
            expected.Ports.Add("direct", 11210);

            Assert.AreEqual(nodesCount, _clusterMap.Nodes.Count);
           
            var actual = _clusterMap.Nodes.Find(x => x.ThisNode);
            Assert.AreEqual(expected.CouchAPIBase, actual.CouchAPIBase);
            Assert.AreEqual(expected.ClusterCompatibility, actual.ClusterCompatibility);
            Assert.AreEqual(expected.ClusterMembership, actual.ClusterMembership);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.CouchAPIBase, actual.CouchAPIBase);
        }

        [Test]
        public void TestDDocs()
        {
            const int ddocsCount = 1;
            Assert.AreEqual(ddocsCount, _clusterMap.DDocs.Count);
            Assert.AreEqual(_clusterMap.DDocs["uri"], "/pools/default/buckets/default/ddocs");
        }

        [Test]
        public void TestControllers()
        {
            const int controllersCount = 4;
            Assert.AreEqual(controllersCount, _clusterMap.Controllers.Count);
            Assert.AreEqual(_clusterMap.Controllers["compactAll"], "/pools/default/buckets/default/controller/compactBucket");
            Assert.AreEqual(_clusterMap.Controllers["compactDB"], "/pools/default/buckets/default/controller/compactDatabases");
            Assert.AreEqual(_clusterMap.Controllers["purgeDeletes"], "/pools/default/buckets/default/controller/unsafePurgeBucket");
            Assert.AreEqual(_clusterMap.Controllers["startRecovery"], "/pools/default/buckets/default/controller/startRecovery");
        }

        [Test]
        public void TestVBucketServerMap()
        {
            var expected = new VBucketServerMap
            {
                HashAlgorithm = "CRC",
                NumReplicas = 1,
                ServerList = new List<string>{"192.168.56.102:11210", "192.168.56.101:11210"},
            };

            Assert.AreEqual(expected.HashAlgorithm, _clusterMap.VBucketServerMap.HashAlgorithm);
            Assert.AreEqual(expected.NumReplicas, _clusterMap.VBucketServerMap.NumReplicas);
            Assert.AreEqual(expected.ServerList.Count, _clusterMap.VBucketServerMap.ServerList.Count);
            Assert.AreEqual(1024, _clusterMap.VBucketServerMap.VBucketMap.Count());
        }
    }
}
