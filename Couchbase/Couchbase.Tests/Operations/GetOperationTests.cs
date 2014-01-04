using System;
using System.Net.Sockets;
using Couchbase.IO;
using Couchbase.IO.Operations;
using NUnit.Framework;

namespace Couchbase.Tests.Operations
{
    [TestFixture]
    public class GetOperationTests : OperationTestsBase
    {
        [Test]
        public void TestGet()
        {
            var operation = new GetOperation<string>(Key, _vBucket);

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
                    var operationResult = operation.GetResult();
                    Console.WriteLine(operationResult.Message);
                    Console.WriteLine(operationResult.Value);
                }
            }
            finally
            {
                _connectionPool.Release(connection);
            }
        }

        [Test]
        public void _When_Get_Is_Called_With_Invalid_Key_What_Happens()
        {
            const string keyThatDoesNotExist = "barfkey3";
            var operation = new GetOperation<string>(keyThatDoesNotExist, _vBucket);
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
                    Console.WriteLine(result.Value);
                }
            }
            finally
            {
                _connectionPool.Release(connection);
            }
        }
    }
}
