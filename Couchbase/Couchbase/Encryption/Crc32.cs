using System;
using System.Security.Cryptography;

namespace Couchbase.Encryption
{
    public class Crc32 : HashAlgorithm
    {
        private const uint Polynomial = 0xedb88320u;
        private const uint Seed = 0xffffffffu;
        private static readonly uint[] Table = new uint[256];
        private uint hash = 0;

        static Crc32()
        {
            for (var i = 0u; i < Table.Length; ++i)
            {
                var temp = i;
                for (var j = 8u; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = ((temp >> 1) ^ Polynomial);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                Table[i] = temp;
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = Seed;
            for (int i = ibStart; i < cbSize - ibStart; i++)
            {
                hash = (hash >> 8) ^ Table[array[i] ^ hash & 0xff];
            }
        }

        protected override byte[] HashFinal()
        {
            return ConvertBytesToBigEndian(~hash);
        }

        public override void Initialize()
        {
            hash = Seed;
        }

        private byte[] ConvertBytesToBigEndian(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
    }
}
