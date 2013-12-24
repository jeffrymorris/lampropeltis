using System;

namespace Couchbase.IO.Operations
{
    public struct OperationHeader
    {
        public int Magic { get; set; }

        public OpCode OpCode { get; set; }

        public string Key { get; set; }

        public int ExtrasLength { get; set; }

        public TypeCode DataType { get; set; }

        public ResponseStatus Status { get; set; }

        public int KeyLength { get; set; }

        public int BodyLength { get; set; }

        public uint Opaque { get; set; }

        public ulong Cas { get; set; }

        public bool HasData()
        {
            return BodyLength > 0;
        }
    }
}
