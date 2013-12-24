
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
