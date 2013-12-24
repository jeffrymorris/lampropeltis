using System.Collections.Generic;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;

namespace Couchbase
{
    public interface IClusterState
    {
        SortedList<int, IConfigProvider> ConfigProviders { get; }

        ICouchbaseClientConfig Config { get; }

        void Initialize();

        INode GetServerForKey(string key);
        IVBucket GetVBucket(string key);
    }
}
