
namespace Couchbase.IO.Operations
{
    public class SetOperation<T> 
        : OperationBase<T>
    {
        public SetOperation(string key, T value, IVBucket vBucket) 
            : base(key, value, vBucket)
        {
        }

        public override OpCode OpCode
        {
            get { return OpCode.Set; }
        }
    }
}
