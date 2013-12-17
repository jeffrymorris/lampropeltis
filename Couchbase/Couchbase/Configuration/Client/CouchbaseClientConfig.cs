
namespace Couchbase.Configuration.Client
{
    public class CouchbaseClientConfig : ICouchbaseClientConfig
    {
        private readonly IConnectionPoolConfig _connectionPoolConfig;
        private readonly IBootstrapConfig _bootstrapConfig;
        private readonly IConfigProviderConfig _configProviderConfig;
        private readonly string _defaultPath;

        public CouchbaseClientConfig(string defaultPath)
        {
            _defaultPath = defaultPath;
            _connectionPoolConfig = new ConnectionPoolConfig();
            _bootstrapConfig = new BootstrapConfig();
            _configProviderConfig = new ConfigProviderConfig(this);
        }

        public CouchbaseClientConfig(IConnectionPoolConfig connectionPoolConfig, IBootstrapConfig bootstrapConfig, IConfigProviderConfig configProviderConfig)
        {
            _connectionPoolConfig = connectionPoolConfig;
            _bootstrapConfig = bootstrapConfig;
            _configProviderConfig = configProviderConfig;
        }

        public IConnectionPoolConfig ConnectionPoolConfiguration
        {
            get { return _connectionPoolConfig; }
        }

        public IBootstrapConfig BootstrapConfiguration
        {
            get { return _bootstrapConfig; }
        }

        public IConfigProviderConfig ConfigProviderConfig
        {
            get { return _configProviderConfig; }
        }


        public string DefaultPath
        {
            get { return _defaultPath; }
        }
    }
}
