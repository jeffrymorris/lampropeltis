using System;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;
using Couchbase.IO.Operations;

namespace Couchbase
{
    public class CouchbaseClient : IClient
    {
        private readonly IClusterState _clusterState;
        private readonly IConfigProvider _configProvider;
        private readonly ICouchbaseClientConfig _couchbaseClientConfig;

        public CouchbaseClient(ICouchbaseClientConfig couchbaseClientConfig)
        {
            _couchbaseClientConfig = couchbaseClientConfig;
            _clusterState = new ClusterState(couchbaseClientConfig);
        }

        public IOperationResult Get(string key)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Set(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
