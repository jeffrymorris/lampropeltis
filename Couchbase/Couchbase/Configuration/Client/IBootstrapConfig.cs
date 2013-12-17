using System;
using System.Collections.Generic;

namespace Couchbase.Configuration.Client
{
    public interface IBootstrapConfig
    {
        List<Uri> BootstrapUrls { get; set; } 

        int RetryAttempts { get; set; }
    }
}
