
namespace Couchbase.Configuration.Client
{
    public interface IConnectionPoolConfig
    {
        int MaxSize { get; }

        int MinSize { get; }

        int WaitTimeout { get; }

        int RecieveTimeout { get; }

        int SendTimeout { get; }

        int ShutdownTimeout { get; }
    }
}
