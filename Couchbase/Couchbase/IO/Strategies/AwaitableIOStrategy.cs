using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.IO.Operations;
using Couchbase.IO.Utils;

namespace Couchbase.IO.Strategies
{
    public class AwaitableIOStrategy : IOStrategy
    {
        private readonly IConnectionPool _connectionPool;
        private readonly SocketAsyncEventArgsPool _asyncEventArgsPool;

        public AwaitableIOStrategy(IConnectionPool connectionPool, SocketAsyncEventArgsPool asyncEventArgsPool)
        {
            _connectionPool = connectionPool;
            _asyncEventArgsPool = asyncEventArgsPool;
        }

        public async Task<IOperationResult<T>> ExecuteAsync<T>(IOperation<T> operation)
        {
            await Send(operation);
            return operation.GetResult();
        }

        public IOperationResult<T> Execute<T>(IOperation<T> operation)
        {
            Send(operation);
            return operation.GetResult();
        }

        async Task Send<T>(IOperation<T> operation)
        {
            var connection = _connectionPool.Acquire();
            var state = new OperationAsyncState
            {
                Connection = connection,
                OperationId = operation.SequenceId
            };

            var buffer = operation.GetBuffer();
            var receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.UserToken = state;
            receiveEventArgs.SetBuffer(buffer, 0, buffer.Length);

            Console.WriteLine("Send Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            var awaitable = new SocketAwaitable(receiveEventArgs);
            await state.Connection.Handle.SendAsync(awaitable);
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
                await state.Connection.Handle.ReceiveAsync(awaitable);
                state.BytesSent += args.BytesTransferred;
                state.Data.Write(state.Buff, 0, args.BytesTransferred);
                args.SetBuffer(state.Buff, 0, state.Buff.Length);
                ProcessData(operation, state);
            } while (state.BytesSent < state.TotalLength);
            _connectionPool.Release(state.Connection);
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
                Data = new ArraySegment<byte>(buffer, OperationBase<T>.HeaderLength, state.BodyLength),
            };
            operation.Header = header;
            operation.Body = body;
        }
    }
}
