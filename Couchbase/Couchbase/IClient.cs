using Couchbase.IO.Operations;

namespace Couchbase
{
    public interface IClient
    {
        IOperationResult<T> Get<T>(string key);

        IOperationResult<T> Set<T>(string key, T value);
    }
}
