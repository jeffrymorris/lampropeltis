using Couchbase.IO.Operations;

namespace Couchbase.Serialization
{
    public interface ITypeSerializer
    {
        byte[] Serialize<T>(OperationBase<T> operation);

        T Deserialize<T>(OperationBase<T> operation);
    }
}
