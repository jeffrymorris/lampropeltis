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
