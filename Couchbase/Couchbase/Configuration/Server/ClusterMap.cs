using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Couchbase.Configuration.Server
{
    internal class ClusterMap : IClusterMap
    {
        public ClusterMap()
        {
            Controllers = new Dictionary<string, Uri>();
            Stats = new Dictionary<string, Uri>();
            DDocs = new Dictionary<string, Uri>();
        }
        public string Name { get; set; }

        public string BucketType { get; set; }

        public string AuthType { get; set; }

        public string SaslPassword { get; set; }

        public int ProxyPort { get; set; }

        public bool ReplicaIndex { get; set; }

        public Uri Uri { get; set; }

        public Uri StreamingUri { get; set; }

        public Uri LocalRandomKeyUri { get; set; }

        public Dictionary<string, Uri> Controllers { get; private set; }

        [JsonConverter (typeof(NodeTypeConverter))]
        public List<Node> Nodes { get; set; }

        public Dictionary<string, Uri> Stats { get; private set; }

        public Dictionary<string, Uri> DDocs { get; private set; }

        public string NodeLocator { get; set; }

        public bool AutoCompactionSettings { get; set; }

        public bool FastWarmupSettings { get; set; }

        public string Uuid { get; set; }

        [JsonConverter(typeof(VBucketServerMapConverter))]
        public IVBucketServerMap VBucketServerMap { get; set; }

        public string BucketCapabilitiesVer { get; set; }

        public List<string> BucketCapabilities { get; private set; }
    }
}
