using System.Collections.Generic;

namespace Couchbase.Configuration.Server
{
    public interface IVBucketServerMap
    {
        string HashAlgorithm { get; set; }

        int NumReplicas { get; set; }

        List<string> ServerList { get; set; } 

        int[][] VBucketMap { get; set; } 
    }
}
