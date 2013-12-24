using System;

namespace Couchbase.IO.Operations
{
    public class GetOperation<T> : OperationBase<T>
    {
        public GetOperation(string key, IVBucket vBucket) 
            : base(key, vBucket)
        {
        }

        public override OpCode OpCode
        {
            get { return OpCode.Get; }
        }

        public override ArraySegment<byte> CreateBody()
        {
            return new ArraySegment<byte>(new byte[0x00]);
        }

        public override ArraySegment<byte> CreateExtras()
        {
            return new ArraySegment<byte>(new byte[0x00]);
        }
    }
}
