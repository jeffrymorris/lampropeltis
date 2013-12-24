using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.IO;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class DefaultConnectionPoolTests
    {
        private IConnectionPool _connectionPool;
        private IConnectionPoolConfig _config;
        const int MinSize = 1;
        const int MaxSize = 10;
        const int WaitTimeout = 100;
        private const int ShutdownTimeout = 10000;
        private const int RecieveTimeout = 1000;
        private const int SendTimeout = 1000;
        private const string Server = "127.0.0.1:11210";

        [SetUp]
        public void SetUp()
        {
            var ipEndpoint = Node.GetEndPoint(Server);
            var factory = ConnectionFactory.GetDefault();
            _config = new ConnectionPoolConfig(MaxSize, MinSize, WaitTimeout, RecieveTimeout, ShutdownTimeout, SendTimeout);
            _connectionPool = new DefaultConnectionPool(_config, ipEndpoint, factory);
            _connectionPool.Initialize();
        }


        [Test]
        public void Test_Ctor()
        {
            Assert.AreEqual(MinSize, _connectionPool.Count());
        }

        [Test]
        public void Test_Acquire()
        {
            var connection = _connectionPool.Acquire();
            Assert.IsNotNull(connection);
        }

        [Test]
        public void When_Acquire_Called_Socket_Is_Connected()
        {
            var connection = _connectionPool.Acquire();
            Assert.IsTrue(connection.Handle.Connected);
            Assert.IsNotNull(connection);
        }


        [Test]
        public void Test_Acquire_Unhappy()
        {
            _count = 0;
            while (_count < 1000)
            {
                Task.Run(async () =>
                {
                    await DoShit();
                });
                _count++;
            }

            Thread.Sleep(10000);
        }

        private static int _count;
        async Task DoShit()
        {
            await Task.Run(() =>
            {
                try
                {
                    var connection = _connectionPool.Acquire();
                    if (_count % 3 == 0) Thread.Sleep(10);
                    Assert.IsNotNull(connection);
                    Interlocked.Increment(ref _count);
                    if(_count % 2==0)Thread.Sleep(10);
                    _connectionPool.Release(connection);
                    Console.WriteLine("Count: {0}", _connectionPool.Count());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}
