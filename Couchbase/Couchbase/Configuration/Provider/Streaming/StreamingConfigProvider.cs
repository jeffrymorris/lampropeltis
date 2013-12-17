using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
