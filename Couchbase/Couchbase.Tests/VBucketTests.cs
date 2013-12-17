using System.Collections.Generic;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class VBucketTests
    {
        private IVBucket _vBucket;

        [SetUp]
        public void SetUp()
        {
            var cluster = new List<INode>
            {
                new Node("192.168.56.102:11210"),
                new Node("192.168.56.101:11210")
            };
            _vBucket = new VBucket(cluster, 100, 0, 1);
        }

        [Test]
        public void Test_LocatePrimary()
        {
            var server = _vBucket.LocatePrimary();
            Assert.AreEqual(server.EndPoint.ToString(), "192.168.56.102:11210");
        }

        [Test]
        public void Test_LocateReplica()
        {
            var server = _vBucket.LocateReplica();
            Assert.AreEqual(server.EndPoint.ToString(), "192.168.56.101:11210");
        }

        [Test]
        public void Test_Replica()
        {
            Assert.AreEqual(1, _vBucket.Replica);
        }

        [Test]
        public void Test_Primary()
        {
            Assert.AreEqual(0, _vBucket.Primary);
        }

        [Test]
        public void Test_Index()
        {
            Assert.AreEqual(100, _vBucket.Index);
        }
    }
}
