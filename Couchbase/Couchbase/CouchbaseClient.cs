using Couchbase.Configuration.Client;
using Couchbase.IO.Operations;

namespace Couchbase
{
    public class CouchbaseClient : IClient
    {
        private readonly IClusterState _clusterState;
        private readonly ICouchbaseClientConfig _couchbaseClientConfig;

        public CouchbaseClient(ICouchbaseClientConfig couchbaseClientConfig)
        {
            _couchbaseClientConfig = couchbaseClientConfig;
            _clusterState = new ClusterState(couchbaseClientConfig);
        }

        public IOperationResult<T> Get<T>(string key)
        {
            var vBucket = _clusterState.GetVBucket(key);
            var node = vBucket.LocatePrimary();
            var operation = new GetOperation<T>(key, vBucket);
            return node.Send(operation);
        }

        public IOperationResult<T> Set<T>(string key, T value)
        {
            var vBucket = _clusterState.GetVBucket(key);
            var node = vBucket.LocatePrimary();
            var operation = new SetOperation<T>(key, value, vBucket);
            return node.Send(operation);
        }
    }
}
