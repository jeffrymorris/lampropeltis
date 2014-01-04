using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.IO;
using Couchbase.IO.Operations;
using Couchbase.IO.Utils;
using Couchbase.Tests.Operations;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class SocketAwaitableTests 
    {
        private Socket _socket;
        private SocketAsyncEventArgs _asyncEventArgs;
        private IPEndPoint _endPoint;
        private static AutoResetEvent _autoConnectEvent = new AutoResetEvent(false);

        private IClusterState _state;
        private const string DefaultPath = @"Data\\Configs\\localhost-clustermap-default.json";
        private readonly ICouchbaseClientConfig _defaultClientConfig = new CouchbaseClientConfig(DefaultPath);

        protected const string Key = "Hello";
        protected const string Value = "World";

        [TestFixtureSetUp]
        public void SetUp()
        {
            IPAddress ipAddress;
            IPAddress.TryParse("127.0.0.1", out ipAddress);
            _endPoint = new IPEndPoint(ipAddress, 11210);

            _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream,ProtocolType.Tcp);
            _state = new ClusterState(_defaultClientConfig);
        }
        
        [Test]
        public void TestSet()
        {
            
            Connect();
            var vBucket = _state.GetVBucket(Key);
            Send(new SetOperation<string>(Key, Value, vBucket));

        }

        [Test]
        public async void TestGet()
        {

            Connect();
            var vBucket = _state.GetVBucket(Key);
            var operation = new GetOperation<string>(Key, vBucket);
            await Send(operation);
            Console.WriteLine(operation.Value);
            //Console.WriteLine(Encoding.UTF8.GetString(operation.Body.Data.Array, 24, 8));
        }

        [Test]
        public async void TestGet2()
        {
            Connect();
            var vBucket = _state.GetVBucket(Key);

            for (var i = 0; i < 100; i++)
            {
                var operation = new GetOperation<string>(Key, vBucket);
                await Send(operation);
                Console.WriteLine(operation.Value);
            }
        }

        async Task Send<T>(IOperation<T> operation)
        {
            var state = new OperationAsyncState
            {
                Connection = _socket,
                OperationId = operation.SequenceId
            };

            var buffer = operation.GetBuffer();
            var receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.UserToken = state;
            receiveEventArgs.SetBuffer(buffer, 0, buffer.Length);

            Console.WriteLine("Send Thread: {0}",Thread.CurrentThread.ManagedThreadId);
            var awaitable = new SocketAwaitable(receiveEventArgs);
            await _socket.SendAsync(awaitable);
            await Receive(operation, state);
        }

        async Task Receive<T>(IOperation<T> operation, OperationAsyncState state)
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(state.Buff, 0, state.Buff.Length);
            var awaitable = new SocketAwaitable(args);

            do
            {
                Console.WriteLine("Receive Thread: {0}", Thread.CurrentThread.ManagedThreadId);
                await _socket.ReceiveAsync(awaitable);
                state.BytesSent += args.BytesTransferred;
                state.Data.Write(state.Buff, 0, args.BytesTransferred);
                args.SetBuffer(state.Buff, 0, state.Buff.Length);
                ProcessData(operation, state);
            } while (state.BytesSent < state.TotalLength);
        }

        static void ProcessData<T>(IOperation<T> operation, OperationAsyncState state)
        {
            var buffer = state.Data.GetBuffer();
            var header = new OperationHeader
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
            var body = new OperationBody
            {
                Extras = new ArraySegment<byte>(buffer, 0, header.ExtrasLength),
                Data = new ArraySegment<byte>(buffer, header.ExtrasLength, buffer.Length - header.BodyLength),
            };
            operation.Header = header;
            operation.Body = body;
        }

        static OperationHeader ProcessData(OperationAsyncState state)
        {
            var buffer = state.Data.GetBuffer();
            state.BodyLength = buffer.GetInt32(HeaderIndexFor.Body);
            var header = new OperationHeader
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

            if (state.BytesSent == header.BodyLength + 24)
            {
                Console.WriteLine("Processing message----------");
                Console.WriteLine("Total bytes sent: {0}", state.BytesSent);
                Console.WriteLine("Header=>{0}", header.Magic);
                Console.WriteLine("OpCode=>{0}", header.OpCode);
                Console.WriteLine("KeyLength=>{0}", header.KeyLength);
                Console.WriteLine("ExtrasLength=>{0}", header.ExtrasLength);
                Console.WriteLine("Status=>{0}", header.Status);
                Console.WriteLine("BodyLength=>{0}", header.BodyLength);
                Console.WriteLine("Opaque=>{0}", header.Opaque);
                Console.WriteLine("Cas=>{0}", header.Cas);
                Console.WriteLine("\n\n");

                var result = Encoding.UTF8.GetString(buffer, 24, header.BodyLength);
                Console.WriteLine("Shut the front door! {0}", result);
            }
            return header;
        }

        void Connect()
        {
            _asyncEventArgs = new SocketAsyncEventArgs();
            _asyncEventArgs.RemoteEndPoint = _endPoint;
            _asyncEventArgs.SetBuffer(new byte[0x00], 0, 0);
            _socket.ConnectAsync(_asyncEventArgs);

            var error = _asyncEventArgs.SocketError;
            if (error != SocketError.Success)
            {
                Console.WriteLine(error);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(10000);
            _socket.Dispose();
        }
    }
}
