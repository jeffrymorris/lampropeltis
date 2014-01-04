
using System;
using System.Collections.Generic;
using Couchbase.Serialization;

namespace Couchbase.IO.Operations
{
    public interface IOperation<out T>
    {
        OpCode OpCode { get; }
        OperationHeader Header { get; set; }
        OperationBody Body { get; set; }
        List<ArraySegment<byte>> CreateBuffer();
        byte[] GetBuffer();
        IOperationResult<T> GetResult();
        ITypeSerializer Serializer { get; }
        int SequenceId { get; }
    }
}
