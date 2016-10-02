using enBask.Core.Shared.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.Shared.Clients
{
    public class GameManagerClient : IGameManager
    {
        private Uri _serviceUri;
        private FabricClient _fabricClient;

        public GameManagerClient()
        {
            _fabricClient = new FabricClient();
            _serviceUri = new Uri("fabric:/enBask.Core.App/GameManager");
        }

        long Hash(string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            const ulong fnv64Offset = 14695981039346656037;
            const ulong fnv64Prime = 0x100000001b3;
            ulong hash = fnv64Offset;

            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= fnv64Prime;
            }

            return (long)hash;
        }

        public async Task<GameSession> Create(string partitionId)
        {
            var proxy = ServiceProxy.Create<IGameManager>(_serviceUri,
                new ServicePartitionKey(Hash(partitionId)));
            return await proxy.Create(partitionId);
        }

        public async Task<bool> Destroy(GameSession session)
        {
            var proxy = ServiceProxy.Create<IGameManager>(_serviceUri,
                    new ServicePartitionKey(Hash(session.PartitionKey)));
            return await proxy.Destroy(session);

        }

        public async Task<GameSession> Find(string gameId)
        {
            var partitions = await _fabricClient.QueryManager.GetPartitionListAsync(_serviceUri);

            GameSession foundSession = null;
            var result = Parallel.ForEach(partitions, (part, state) =>
            {
                var info = part.PartitionInformation as Int64RangePartitionInformation;
                var key = info.LowKey;

                var proxy = ServiceProxy.Create<IGameManager>(_serviceUri,
                  new ServicePartitionKey(key));

                var session = proxy.Find(gameId).GetAwaiter().GetResult();

                if (session != null)
                {
                    foundSession = session;
                    state.Break();
                }
            });


            return foundSession;
        }

        public async Task<List<GameSession>> Query()
        {
            var partitions = await _fabricClient.QueryManager.GetPartitionListAsync(_serviceUri);

            var sessions = partitions
                .AsParallel()
                .Select((part) =>
                    {
                        var info = part.PartitionInformation as Int64RangePartitionInformation;
                        var key = info.LowKey;

                        var proxy = ServiceProxy.Create<IGameManager>(_serviceUri,
                          new ServicePartitionKey(key));

                        var result = proxy.Query().GetAwaiter().GetResult();
                        return result;
                    })
                .Aggregate(new List<GameSession>(), (seed, result) => { seed.AddRange(result); return seed; })
                .ToList();

            return sessions;
        }
    }
}
