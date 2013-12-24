using System;

namespace Couchbase.Configuration.Provider.Streaming
{
    public class StreamingConfigProvider : IConfigProvider
    {
        public IConfigInfo GetCached()
        {
            throw new NotImplementedException();
        }

        public IConfigInfo Get()
        {
            throw new NotImplementedException();
        }

        public Server.IClusterMap ClusterMap
        {
            get { throw new NotImplementedException(); }
        }
    }
}
