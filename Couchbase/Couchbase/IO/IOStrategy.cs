using System.Threading.Tasks;
using Couchbase.IO.Operations;

namespace Couchbase.IO
{
    public interface IOStrategy
    {
        IOperationResult<T> Execute<T>(IOperation<T> operation);
    }
}
