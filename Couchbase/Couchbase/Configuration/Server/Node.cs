using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Configuration.Server
{
    public class Node : INode
    {
        public Node()
        {
            Ports = new Dictionary<string, int>();
        }

        public Uri CouchAPIBase { get; set; }

        public string Replication { get; set; }

        public bool ThisNode { get; set; }

        public string ClusterMembership { get; set; }

        public string Status { get; set; }

        public string Hostname { get; set; }

        public int ClusterCompatibility { get; set; }

        public string Version { get; set; }

        public string OS { get; set; }

        public Dictionary<string, int> Ports { get; private set; }
    }
}
