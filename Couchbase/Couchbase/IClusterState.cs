﻿using System.Collections.Generic;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;

namespace Couchbase
{
    public interface IClusterState
    {
        SortedList<int, IConfigProvider> ConfigProviders { get; }

        ICouchbaseClientConfig Config { get; }

        void Initialize();

        IServer GetServerForKey(string key);
    }
}