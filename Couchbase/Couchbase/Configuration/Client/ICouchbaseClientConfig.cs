
namespace Couchbase.Configuration.Client
{
    public interface ICouchbaseClientConfig
    {
        IConnectionPoolConfig ConnectionPoolConfiguration { get; }

        IBootstrapConfig BootstrapConfiguration { get; }

        IConfigProviderConfig ConfigProviderConfig { get; }

        //TODO REFACTOR! move to provider configuration?
        string DefaultPath { get; }
    }
}
