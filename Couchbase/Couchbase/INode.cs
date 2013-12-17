using System;
using System.Collections.Generic;
using System.Net;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace Couchbase
{
    public interface INode : IDisposable
    {
        IPEndPoint EndPoint { get; }

        IOperationResult Send(IOperation operation);

        IConnectionPool ConnectionPool { get; }

        List<IBucket> Buckets { get; }
    }
}
