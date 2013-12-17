using System.Configuration;

namespace Couchbase.Configuration.Client
{
    public class ConnectionPoolConfig : ConfigurationElement, IConnectionPoolConfig
    {
        private int _minSize;
        private int _maxSize;
        private int _waitTimeout;
        private int _receiveTimeout;
        private int _shutdownTimeout;
        private int _sendTimeout;

        public ConnectionPoolConfig()
        {
            _maxSize = 2;
            _minSize = 1;
            _waitTimeout = 2500;
            _receiveTimeout = 2500;
            _shutdownTimeout = 10000;
            _sendTimeout = 2500;
        }

        public ConnectionPoolConfig(int maxSize, int minSize, int waitTimeout, int receiveTimeout, int shutdownTimeout, int sendTimeout)
        {
            //todo enable app.config
            _maxSize = maxSize;
            _minSize = minSize;
            _waitTimeout = waitTimeout;
            _receiveTimeout = receiveTimeout;
            _shutdownTimeout = shutdownTimeout;
            _sendTimeout = sendTimeout;
        }

        public int MaxSize
        {
            get { return _maxSize; }
        }

        public int MinSize
        {
            get { return _minSize; }
        }

        public int WaitTimeout
        {
            get { return _waitTimeout; }
        }

        public int RecieveTimeout
        {
            get { return _receiveTimeout; }
        }

        public int ShutdownTimeout
        {
            get { return _shutdownTimeout; }
        }

        public int SendTimeout
        {
            get { return _sendTimeout; }
        }
    }
}
