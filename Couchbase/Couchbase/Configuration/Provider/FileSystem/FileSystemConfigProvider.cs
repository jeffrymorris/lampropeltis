using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Server;
using Couchbase.Encryption;
using Couchbase.IO;
using Newtonsoft.Json;

namespace Couchbase.Configuration.Provider.FileSystem
{
    public class FileSystemConfigProvider : IConfigProvider
    {
        private IClusterMap _clusterMap;
        private IConfigInfo _configInfo;
        private readonly ICouchbaseClientConfig _clientConfig;
        private readonly Func<IConnectionPool, IConnection> _factory;

        public FileSystemConfigProvider(IClusterMap clusterMap, ICouchbaseClientConfig clientConfig, Func<IConnectionPool, IConnection> factory) 
            : this(clientConfig, factory)
        {
            _clusterMap = clusterMap;
        }

        public FileSystemConfigProvider(ICouchbaseClientConfig clientConfig, Func<IConnectionPool, IConnection> factory)
        {
            _clientConfig = clientConfig;
            _factory = factory;
        }

        void LoadFromDisk()
        {
            using (var stream = File.Open(_clientConfig.DefaultPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    _clusterMap = JsonConvert.DeserializeObject<ClusterMap>(reader.ReadToEnd());
                }
            }
        }

        void CreateConfig()
        {
            var vBucketServerMap = _clusterMap.VBucketServerMap;
            var servers = vBucketServerMap.
                ServerList.Select(CreateServer).
                ToList();

            var vBuckets = new Dictionary<int, IVBucket>();
            for (var i = 0; i < vBucketServerMap.VBucketMap.Length; i++)
            {
                var primary = vBucketServerMap.VBucketMap[i][0];
                var replica = vBucketServerMap.VBucketMap[i][1];
                var vBucket = new VBucket(servers, i, primary, replica);
                vBuckets[i] = vBucket;
                Console.WriteLine("Index=>{0} : [{1}, {2}]", i, primary, replica);
            }

            //TODO make injectable via Func<>?
            _configInfo = new DefaultConfig(new Crc32(), servers, vBuckets);
        }

        IServer CreateServer(string server)
        {
            var connectionPool = new DefaultConnectionPool(_clientConfig.ConnectionPoolConfiguration, _factory);
            return new Couchbase.Server(server, connectionPool);
        }

        public IConfigInfo GetCached()
        {
            if (_configInfo == null)
            {
                LoadFromDisk();
                CreateConfig();
            }
            else
            {
                var lastUpdated = File.GetLastWriteTime(_clientConfig.DefaultPath);
                if (lastUpdated > _configInfo.CreationTime)
                {
                    LoadFromDisk();
                    CreateConfig();
                }
            }
            return _configInfo;
        }

        public IConfigInfo Get()
        {
            throw new NotImplementedException();
        }


        public IClusterMap ClusterMap
        {
            get { return _clusterMap; }
        }
    }
}
