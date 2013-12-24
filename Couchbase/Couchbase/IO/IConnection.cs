using System;
using System.Net.Sockets;

namespace Couchbase.IO
{
    public interface IConnection : IDisposable
    {
        Socket Handle { get; }

        Guid Identity { get; }

        //void Execute(IOperation operation);
    }
}
