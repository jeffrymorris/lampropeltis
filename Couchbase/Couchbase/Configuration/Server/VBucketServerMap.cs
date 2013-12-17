using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
