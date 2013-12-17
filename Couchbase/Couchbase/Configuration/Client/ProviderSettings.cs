using System;
using System.Collections.Generic;

namespace Couchbase.Configuration.Client
{
    public class ProviderSettings
    {
        public ProviderSettings()
        {
            Parameters = new Dictionary<string, object>();
        }

        public Type ProviderType { get; set; }

        public Dictionary<string, object> Parameters { get; set; }
    }
}
