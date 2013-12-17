using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Server;

namespace Couchbase.Configuration
{
    public interface IConfigInfo
    {
        IVBucket HashToVBucket(string key);

        DateTime CreationTime { get; }

        List<IServer> GetServers();
    }
}
