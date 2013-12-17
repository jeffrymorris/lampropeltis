using System.Collections.Generic;

namespace Couchbase
{
    public interface IVBucket
    {
        IServer LocatePrimary();

        IServer LocateReplica();

        List<IServer> Replicas { get; }

        int Index { get; }

        int Primary { get; }

        int Replica { get; }
    }
}
