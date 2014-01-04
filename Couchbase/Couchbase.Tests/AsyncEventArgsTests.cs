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
    public class AsyncEventArgsTests 
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
        public void TestGet()
        {

            Connect();
            var vBucket = _state.GetVBucket(Key);
            Send(new GetOperation<string>(Key, vBucket));
        }

        void Send<T>(IOperation<T> operation)
        {
            var state = new OperationAsyncState
            {
                Connection = _socket,
                OperationId = operation.SequenceId
            };
            var buffer = operation.GetBuffer();
            _asyncEventArgs.UserToken = state;
            _asyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
            var result = _socket.SendAsync(_asyncEventArgs);
        }

        void Connect()
        {
            _asyncEventArgs = new SocketAsyncEventArgs();
            _asyncEventArgs.RemoteEndPoint = _endPoint;
            _asyncEventArgs.Completed += OnConnect;
            _asyncEventArgs.SetBuffer(new byte[0x00], 0, 0);
            _socket.ConnectAsync(_asyncEventArgs);
           // _autoConnectEvent.WaitOne();

            var error = _asyncEventArgs.SocketError;
            if (error != SocketError.Success)
            {
                Console.WriteLine(error);
            }
        }

        void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            //_autoConnectEvent.Set();
            Console.WriteLine("{0}=>{1}",e.SocketError, e.LastOperation);
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessRecieve3(e);
                    break;
                case SocketAsyncOperation.Send:
                    var result= _socket.ReceiveAsync(e);
                    if (result)
                    {
                        Console.WriteLine("working");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ProcessRecieve3(SocketAsyncEventArgs args)
        {
            Console.WriteLine("Recieve");
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                Console.WriteLine("Recieving {0} bytes", args.BytesTransferred);
                var state = args.UserToken as OperationAsyncState;
                if (state == null)
                {
                    throw new ArgumentNullException("args");
                }
                
                state.BytesSent += args.BytesTransferred;
                var offset = 0;
                state.Data.Write(args.Buffer, offset, args.BytesTransferred);
                args.SetBuffer(state.Buff, 0, state.Buff.Length);

                OperationHeader header;
                lock (this)
                {
                    
                   header = ProcessData(state);
                }

                if (state.BytesSent < header.BodyLength + 24)
                {
                    var result = _socket.ReceiveAsync(args);
                    Console.WriteLine("Result=> {0}", result);
                }
                else
                {
                    var buffer = state.Data.GetBuffer();

                    var result = Encoding.UTF8.GetString(buffer, 24, header.BodyLength);
                    Console.WriteLine("Shut the front door! {0}", result);
                }
            }
        }

        static OperationHeader ProcessData(OperationAsyncState state)
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
            }
            return header;
        }

        void ProcessRecieve2(SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                var state = args.UserToken as OperationAsyncState;
                if(state == null) throw new ArgumentNullException("args");

                var buffer = args.Buffer;
                if (state.Buffer == null) //need to get a buffer
                {
                    state.Buffer = new byte[args.BytesTransferred];
                    buffer.CopyTo(state.Buffer, 0);
                }
                else
                {
                    var temp = state.Buffer;
                    Array.Resize(ref temp, args.BytesTransferred+temp.Length);
                    buffer.CopyTo(temp, buffer.Length);
                }
                if (!_socket.ReceiveAsync(args))
                {
                    ProcessRecieve2(args);
                }
            }
        }

        void ProcessRecieve(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                var state = args.UserToken as OperationAsyncState;

                var buffer = args.Buffer;
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

                if (header.HasData())
                {
                    
                    var bodyArgs = new SocketAsyncEventArgs();
                    var bodyBuf = new byte[header.BodyLength-header.ExtrasLength];
                    
                    bodyArgs.SetBuffer(bodyBuf, 0, bodyBuf.Length);
                    bodyArgs.Completed += bodyArgs_Completed;
                    bodyArgs.AcceptSocket = _socket;
                    _socket.ReceiveAsync(bodyArgs);
                }
            }
            else
            {
                throw new SocketException((int)args.SocketError);
            }
        }

        void bodyArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var body = Encoding.UTF8.GetString(e.Buffer, 0, e.Buffer.Length-1);
            Console.WriteLine(body);
        }

        [TearDown]
        public void TearDown()
        {
           
            Thread.Sleep(10000);
        }
    }
}
