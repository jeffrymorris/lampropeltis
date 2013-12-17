using System;
using System.Collections.Generic;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;

namespace Couchbase
{
    public class ClusterState : IClusterState
    {
        private readonly ICouchbaseClientConfig _config;
        private readonly SortedList<int, IConfigProvider> _providers = new SortedList<int, IConfigProvider>();

        public ClusterState(ICouchbaseClientConfig config)
        {
            _config = config;
            Initialize();
        }

        public SortedList<int, IConfigProvider> ConfigProviders
        {
            get { return _providers; }
        }

        public ICouchbaseClientConfig Config
        {
            get { return _config; }
        }

        public void Initialize()
        {
            CreateProviders();
        }

        void CreateProviders()
        {
            var providerTypes = _config.ConfigProviderConfig.Providers;
            foreach (var provider in providerTypes)
            {
                var settings = provider.Value;
                IConfigProvider configProvider;
                if (settings.Parameters.Count > 0)
                {
                    var parameters = new object[settings.Parameters.Count];
                    settings.Parameters.Values.CopyTo(parameters, 0);
                    configProvider = (IConfigProvider) Activator.CreateInstance(settings.ProviderType, parameters);
                }
                else
                {
                    configProvider = (IConfigProvider) Activator.CreateInstance(settings.ProviderType);
                }
                _providers.Add(provider.Key, configProvider);
            }
        }


        public INode GetServerForKey(string key)
        {
            //TODO refactor so not hardcoded - this is just to illustrate/validate
            const int fileSystemProviderIndex = 2;
            var provider = _providers[fileSystemProviderIndex];
            var config = provider.GetCached();
            var vBucket = config.HashToVBucket(key);
            return vBucket.LocatePrimary();
        }
    }
}
