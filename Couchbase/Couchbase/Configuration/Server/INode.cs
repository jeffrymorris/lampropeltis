using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Configuration.Server
{
    public interface INode
    {
        Uri CouchAPIBase { get; set; }

        bool ThisNode { get; set; }

        string Replication { get; set; }

        string ClusterMembership { get; set; }

        string Status { get; set; }

        string Hostname { get; set; }

        int ClusterCompatibility { get; set; }

        string Version { get; set; }

        string OS { get; set; }

        Dictionary<string, int> Ports { get; } 
    }
}
