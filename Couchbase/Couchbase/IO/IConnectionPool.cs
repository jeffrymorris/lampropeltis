using System;
using System.Net;
using Couchbase.Configuration.Client;

namespace Couchbase.IO
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Acquire();

        void Release(IConnection connection);

        int Count();

        IConnectionPoolConfig Config { get; }

        IPEndPoint EndPoint{ get; set; }

        void Initialize();
    }
}
