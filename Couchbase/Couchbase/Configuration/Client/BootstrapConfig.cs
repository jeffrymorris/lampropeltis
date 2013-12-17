using System;
using System.Collections.Generic;

namespace Couchbase.Configuration.Client
{
    public class BootstrapConfig : IBootstrapConfig
    {
        private readonly Uri LocalHost = new Uri("http://localhost:8091/pools");

        public BootstrapConfig()
        {
            BootstrapUrls = new List<Uri>
            {
                LocalHost
            };
            RetryAttempts = 2;
        }
        public BootstrapConfig(List<Uri> urls, int retryAttempts)
        {
            BootstrapUrls = urls;
            RetryAttempts = retryAttempts;
        }
        public List<Uri> BootstrapUrls { get; set; }

        public int RetryAttempts { get; set; }
    }
}
