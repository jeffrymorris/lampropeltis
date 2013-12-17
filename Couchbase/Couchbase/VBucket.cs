using System.Collections.Generic;
using System.Linq;

namespace Couchbase
{
    public class VBucket : IVBucket
    {
        private readonly int _primary;
        private readonly int _replica;
        private readonly int _index;
        private readonly List<INode> _cluster;

        public VBucket(List<INode> cluster, int index, int primary, int replica)
        {
            _cluster = cluster;
            _index = index;
            _primary = primary;
            _replica = replica;
        }

        public INode LocatePrimary()
        {
            return _cluster[Primary];
        }

        public List<INode> Replicas
        {
            get { return _cluster.Skip(1).ToList(); }
        }

        public int Index { get { return _index; } }

        public int Primary { get { return _primary; } }

        public int Replica { get { return _replica; } }

        public INode LocateReplica()
        {
            return _cluster[Replica];
        }
    }
}
