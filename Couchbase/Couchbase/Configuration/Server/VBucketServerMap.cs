using System.Collections.Generic;

namespace Couchbase.Configuration.Server
{
    public class VBucketServerMap : IVBucketServerMap
    {
        public VBucketServerMap()
        {
            ServerList = new List<string>();
        }

        public string HashAlgorithm { get; set; }

        public int NumReplicas { get; set; }

        public List<string> ServerList { get; set; }

        public int[][] VBucketMap { get; set; }
    }
}
