using System;
using System.Collections.Generic;

namespace Couchbase.Configuration
{
    public interface IConfigInfo
    {
        IVBucket HashToVBucket(string key);

        DateTime CreationTime { get; }

        List<INode> GetServers();
    }
}
