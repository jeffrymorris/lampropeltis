using System;
using System.Collections.Generic;

namespace Couchbase.Configuration.Server
{
    public interface IClusterMap
    {
        string Name { get; set; }

        string BucketType { get; set; }

        string AuthType { get; set; }

        string SaslPassword { get; set; }

        int ProxyPort { get; set; }

        bool ReplicaIndex { get; set; }

        Uri Uri { get; set; }

        Uri StreamingUri { get; set; }

        Uri LocalRandomKeyUri { get; set; }

        Dictionary<string, Uri> Controllers { get; } 

        List<Node> Nodes { get; set; } 

        Dictionary<string, Uri> Stats { get; }

        Dictionary<string, Uri> DDocs { get; } 

        string NodeLocator { get; set; }

        bool AutoCompactionSettings { get; set; }

        bool FastWarmupSettings { get; set; }

        string Uuid { get; set; }

        IVBucketServerMap VBucketServerMap { get; set; }

        string BucketCapabilitiesVer { get; set; }

        List<string> BucketCapabilities { get;}
    }
}
