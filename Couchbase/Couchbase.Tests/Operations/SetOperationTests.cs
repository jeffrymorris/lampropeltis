using System;
using System.Net.Sockets;
using Couchbase.IO;
using Couchbase.IO.Operations;
using NUnit.Framework;

namespace Couchbase.Tests.Operations
{
    [TestFixture]
    public class SetOperationTests : OperationTestsBase
    {
        private new const string Key = "MyKey";
        private new const string Value = "MyValue";

        [Test]
        public void TestSet()
        {
            var operation = new SetOperation<string>(Key, Value, _vBucket);

            var buffer = operation.CreateBuffer();
            Assert.AreEqual(buffer.Count, 4);

            IConnection connection = null;
            try
            {
                connection = _connectionPool.Acquire();
                SocketError error;
                connection.Handle.Send(buffer, SocketFlags.None, out error);
                Assert.AreEqual(SocketError.Success, error);

                operation.Header = ReadHeader(connection);
                if (operation.Header.HasData())
                {
                    operation.Body = ReadBody(connection, operation.Header);
                    var result = operation.GetResult();
                    Console.WriteLine(result.Message);
                }
            }
            finally
            {
                _connectionPool.Release(connection);
            }
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            _connectionPool.Dispose();
        }
    }
}
