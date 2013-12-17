
namespace Couchbase.IO.Operations
{
    public interface IOperationResult
    {
        bool Success { get; }

        object Value { get; }

        string Message { get; }
    }
}
