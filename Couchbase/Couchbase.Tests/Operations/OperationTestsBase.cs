using System;
using System.Net.Sockets;
using Couchbase.Configuration;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Provider;
using Couchbase.Configuration.Provider.FileSystem;
using Couchbase.IO;
using Couchbase.IO.Operations;
using Couchbase.IO.Utils;
using NUnit.Framework;

namespace Couchbase.Tests.Operations
{
    public class OperationTestsBase
    {
        protected const string DefaultPath = @"Data\\Configs\\localhost-clustermap-default.json";
        protected const string Server = "127.0.0.1:11210";
        protected readonly ICouchbaseClientConfig _defaultClientConfig = new CouchbaseClientConfig(DefaultPath);
        protected IConfigProvider _provider;
        protected IConfigInfo _configInfo;
        protected IVBucket _vBucket;

        protected IConnectionPool _connectionPool;
        protected IConnectionPoolConfig _config;

        protected const string Key = "MyTestKeyhhhhhhhhhh";
        protected const string Value = "MyTestValue";

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            var factory = ConnectionFactory.GetDefault();
            _connectionPool = new DefaultConnectionPool(_defaultClientConfig.ConnectionPoolConfiguration, Node.GetEndPoint(Server), factory);
            _connectionPool.Initialize();

            _provider = new FileSystemConfigProvider(_defaultClientConfig, ConnectionFactory.GetDefault());
            _configInfo = _provider.GetCached();
            _vBucket = _configInfo.HashToVBucket(Key);
        }

        protected virtual OperationHeader ReadHeader(IConnection connection)
        {
            var header = new ArraySegment<byte>(new byte[24]);
            connection.Handle.Receive(header.Array, 0, header.Array.Length, SocketFlags.None);

            byte[] buffer = header.Array;
            return new OperationHeader
            {
                Magic = buffer[HeaderIndexFor.Magic],
                OpCode = buffer[HeaderIndexFor.Opcode].ToOpCode(),
                KeyLength = buffer.GetInt16(HeaderIndexFor.KeyLength),
                ExtrasLength = buffer[HeaderIndexFor.ExtrasLength],
                Status = buffer.GetResponseStatus(HeaderIndexFor.Status),
                BodyLength = buffer.GetInt32(HeaderIndexFor.Body),
                Opaque = buffer.GetUInt32(HeaderIndexFor.Opaque),
                Cas = buffer.GetUInt64(HeaderIndexFor.Cas)
            };
        }

        protected virtual OperationBody ReadBody(IConnection connection, OperationHeader header)
        {
            var buffer = new byte[header.BodyLength];
            connection.Handle.Receive(buffer, 0, buffer.Length, SocketFlags.None);

            return new OperationBody
            {
                Extras = new ArraySegment<byte>(buffer, 0, header.ExtrasLength),
                Data = new ArraySegment<byte>(buffer, header.ExtrasLength, buffer.Length - header.ExtrasLength),
            };
        }
    }
}