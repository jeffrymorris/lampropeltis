using System.Collections.Generic;
using Couchbase.Configuration.Provider.FileSystem;
using Couchbase.Configuration.Provider.Publication;
using Couchbase.Configuration.Provider.Streaming;
using Couchbase.IO;

namespace Couchbase.Configuration.Client
{
    public class ConfigProviderConfig : IConfigProviderConfig
    {
        private readonly ICouchbaseClientConfig _clientConfig;
        private readonly SortedList<int, ProviderSettings> _providers; 
        
        //TODO refactor so not hardcoded
        //TODO This is a TRAVASHAMOCKERY!
        public ConfigProviderConfig(ICouchbaseClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _providers = new SortedList<int, ProviderSettings>
            {
                {0, new ProviderSettings{ProviderType = typeof(StreamingConfigProvider)}},
                {1, new ProviderSettings{ProviderType = typeof(CccpConfigProvider)}},
                {2, new ProviderSettings{ProviderType = typeof(FileSystemConfigProvider), Parameters =
                {
                    {"config", _clientConfig}, 
                    {"factory", ConnectionFactory.GetDefault()}
                }}}
            };
        }
        public SortedList<int, ProviderSettings> Providers
        {   
            get { return _providers; }
        }
    }
}
