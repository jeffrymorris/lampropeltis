using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace Couchbase.Tests
{
    public class OperationAsyncState
    {
        public const int HeaderLength = 24;

        public int TotalLength { get { return HeaderLength + BodyLength; }}

        public int OperationId { get; set; } 

        public Socket Connection { get; set; }//change to IConnection later

        public byte[] Buff = new byte[512];
        public byte[] Buffer { get; set; }

        public int BodyLength { get; set; }

        public int ExtrasLength { get; set; }

        public int KeyLength { get; set; }

        public MemoryStream Data = new MemoryStream();

        public int BytesSent { get; set; }

        public bool HasBegun { get; set; }
    }
}
