using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.IO.Operations
{
    public interface IOperationRequest
    {
        OpCode OpCode { get; }

        int CorrelationId { get; }

        string Key { get; }

        ulong Cas { get; }

        ushort Reserved { get; }
    }
}
