using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Configuration
{
    public class DefaultConfig : IConfigInfo
    {
        private const int Mask = 1023;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly Dictionary<int, IVBucket> _vBuckets;
        private readonly List<INode> _cluster;
        private readonly DateTime _creationTime = DateTime.Now;

        public DefaultConfig(HashAlgorithm hashAlgorithm, List<INode> cluster, Dictionary<int, IVBucket> vBuckets)
        {
            _hashAlgorithm = hashAlgorithm;
            _vBuckets = vBuckets;
            _cluster = cluster;
        }

        public int GetIndex(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var hashedKeyBytes = _hashAlgorithm.ComputeHash(keyBytes);
            var hash = BitConverter.ToUInt32(hashedKeyBytes, 0);
            return (int)hash & Mask;
        }

        public IVBucket HashToVBucket(string key)
        {
            var index = GetIndex(key);
            return _vBuckets[index];
        }

        public DateTime CreationTime
        {
            get { return _creationTime; }
        }

        public List<INode> GetServers()
        {
            return _cluster.ToList();
        }
    }
}
