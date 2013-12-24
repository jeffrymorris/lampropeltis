using System;
using System.Text;
using Couchbase.Encryption;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class Crc32Tests
    {
        [Test]
        public void Test()
        {
            const string key = "get_unit_test_635223099507033468";
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var crc = new Crc32();
            var bytes = crc.ComputeHash(keyBytes);
            var hash = BitConverter.ToUInt32(bytes, 0);
            var value = hash & 1023;
            Console.WriteLine(hash);
            Console.WriteLine("|");
            Console.WriteLine(value);
        }
    }
}
