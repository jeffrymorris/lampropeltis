using System.Collections.Generic;

namespace Couchbase
{
    public interface IVBucket
    {
        INode LocatePrimary();

        INode LocateReplica();

        List<INode> Replicas { get; }

        int Index { get; }

        int Primary { get; }

        int Replica { get; }
    }
}
