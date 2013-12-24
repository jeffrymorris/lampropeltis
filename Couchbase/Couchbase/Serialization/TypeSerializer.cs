using System;
using System.Text;
using Couchbase.IO.Operations;

namespace Couchbase.Serialization
{
    public sealed class TypeSerializer : ITypeSerializer
    {
        private static readonly byte[] NullArray = new byte[0];

        public byte[] Serialize<T>(OperationBase<T> operation)
        {
            var value = (object)operation.Value;
            var type = typeof (T);
            var typeCode = value == null ? 
                TypeCode.DBNull : 
                Type.GetTypeCode(type);

            byte[] bytes = null;
            switch (typeCode)
            {
                case TypeCode.String:
                    bytes = GetBytes(value as string);
                    break;
                case TypeCode.Int32:
                    bytes = GetBytes(Convert.ToInt32(value));
                    break;
                case TypeCode.DBNull:
                    bytes = GetBytes();
                    break;
            }
            return bytes;
        }

        public T Deserialize<T>(OperationBase<T> operation)
        {
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            var operationBody = operation.Body;
            var data = operationBody.Data;

            object value = null;
            switch (typeCode)
            {
                case TypeCode.String:
                    value = GetString(data);
                    break;
                case TypeCode.Int32:
                    value = GetInt32(data);
                    break;
            }
            return (T)value;
        }

        static byte[] GetBytes()
        {
            return NullArray;
        }

        static byte[] GetBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        static byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        static string GetString(ArraySegment<byte> bytes)
        {
            var result = string.Empty;
            if (bytes.Array != null)
            {
                var index = bytes.Offset;
                var count = bytes.Array.Length - bytes.Offset;
                result = Encoding.UTF8.GetString(bytes.Array, index, count);
            }
            return result;
        }

        static int GetInt32(ArraySegment<byte> bytes)
        {
            var result = 0;
            if (bytes.Array != null)
            {
                result = BitConverter.ToInt32(bytes.Array, bytes.Offset);
            }
            return result;
        }
    }
}
