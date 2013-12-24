using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Couchbase.IO.Operations;
using Couchbase.IO.Utils;

namespace Couchbase.IO
{
    public class Connection : IConnection
    {
        private readonly IConnectionPool _connectionPool;
        private readonly Socket _handle;
        private readonly Guid _identity = Guid.NewGuid();
        private bool _disposed;

        public Connection(IConnectionPool connectionPool, Socket handle)
        {
            _connectionPool = connectionPool;
            _handle = handle;
        }

        public Socket Handle
        {
            get { return _handle; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    if (_handle != null)
                    {
                        if (_handle.Connected)
                        {
                            _handle.Shutdown(SocketShutdown.Both);
                            _handle.Close(_connectionPool.Config.ShutdownTimeout);
                        }
                        else
                        {
                            _handle.Close();
                            _handle.Dispose();
                        }
                    }
                }
            }
            else
            {
                if (_handle != null)
                {
                    _handle.Close();
                    _handle.Dispose();
                }
            }
            _disposed = true;
        }

        ~Connection()
        {
            Dispose(false);
        }

        public Guid Identity
        {
            get { return _identity; }
        }

        public void Execute<T>(IOperation<T> operation)
        {
            throw new NotImplementedException();
        }

        public static async Task ReadAsync(IConnection connection)
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[0x1000],0, 0x1000);
            var awaitable = new SocketAwaitable(args);

            while (true)
            {
                await connection.Handle.ReceiveAsync(awaitable);
                int bytesRead = args.BytesTransferred;
                if (bytesRead <= 0) break;

                Console.WriteLine(bytesRead);
            }
        }

        public static async Task SendAsync(IConnection connection)
        {
            var args = new SocketAsyncEventArgs();
            
        }
    }
}
