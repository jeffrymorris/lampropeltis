using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Server;

namespace Couchbase.Configuration.Provider
{
    public interface IConfigProvider
    {
        IConfigInfo GetCached();

        IConfigInfo Get();

        IClusterMap ClusterMap { get; }
    }
}
