using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.IO.Operations;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class APMTests
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(true);
        private static ManualResetEvent sendDone = new ManualResetEvent(true);
        private static ManualResetEvent receiveDone = new ManualResetEvent(true);

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

            _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _state = new ClusterState(_defaultClientConfig);
        }

        [Test]
        public void TestGet()
        {
            Connect(_endPoint, _socket);

            var vBucket = _state.GetVBucket(Key);
            Send(_socket, new GetOperation<string>(Key, vBucket));
           
            Assert.IsNotNull("shit");
        }

        public static void Connect(EndPoint endpoint, Socket socket)
        {
            socket.BeginConnect(endpoint, OnConnect, socket);
        }

        private static void OnConnect(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                if (socket != null)
                {
                    socket.EndConnect(ar);
                    Console.WriteLine("connected to {0}", socket.RemoteEndPoint);
                }
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Send<T>(Socket socket, IOperation<T> operation)
        {
            var buffers = operation.CreateBuffer();
            socket.BeginSend(buffers, SocketFlags.None, OnSend, socket);
        }

        private static void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                int bytesSent = socket.EndSend(ar);
                Console.WriteLine("Bytes sent: {0}", bytesSent);
                sendDone.Set();
                Recieve(socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Recieve(Socket socket)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = socket;

                IAsyncResult ar = socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnReceive(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as StateObject;
                if (state != null)
                {
                    var socket = state.workSocket;

                    var bytesRead = socket.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
                    }
                    else
                    {
                        if (state.sb.Length > 1)
                        {
                            Console.WriteLine(state.sb);
                        }
                        receiveDone.Set();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(10000);
        }
    }
}
