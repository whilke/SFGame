using enBask.Core.GameManager.interfaces;
using enBask.Core.Shared;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using GameSessionLookupDict = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, enBask.Core.Shared.GameSession>;
using GameSessionsList = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, System.Collections.Generic.List<enBask.Core.Shared.GameSession>>;
namespace enBask.Core.GameManager
{
    public class LifetimeManager : ILifetimeManager
    {
        FabricClient _client;
        IReliableStateManagerReplica _stateMgr;
        public LifetimeManager(IReliableStateManagerReplica stateMgr)
        {
            _stateMgr = stateMgr;
            _client = new FabricClient();
        }
        public async Task<GameSession> CreateAsync(string serviceType, string pKey)
        {

            var service = ServiceDescriptorFactory.Get(serviceType);
            if (service == null) throw new ArgumentException("serviceType can't be found");

            string new_game_id = Guid.NewGuid().ToString().Replace("-", "");

            StatefulServiceDescription sd = new StatefulServiceDescription();
            sd.ApplicationName = service.AppUri;
            sd.HasPersistedState = true;
            sd.MinReplicaSetSize = service.MinReplicaCount;
            sd.TargetReplicaSetSize = service.TargetReplicaCount;
            sd.ServiceTypeName = service.ServiceType;
            sd.ServiceName = service.GenerateServiceUri(new_game_id);
            sd.PartitionSchemeDescription =
                new SingletonPartitionSchemeDescription();
//                new UniformInt64RangePartitionSchemeDescription(1, 0, 0);

            await _client.ServiceManager.CreateServiceAsync(sd);

            GameSession session = new GameSession();
            session.PartitionKey = pKey;
            session.GameId = new_game_id;

            var sessionDict = await _stateMgr.GetOrAddAsync<GameSessionLookupDict>(nameof(GameSessionLookupDict));
            var sessionList = await _stateMgr.GetOrAddAsync<GameSessionsList>(nameof(GameSessionsList));

            using (var tx = _stateMgr.CreateTransaction())
            {
                await sessionDict.TryAddAsync(tx, session.GameId, session);

                List<GameSession> gameList = null;
                var r = await sessionList.TryGetValueAsync(tx, "all_games");
                if (r.HasValue)
                    gameList = r.Value;
                else
                    gameList = new List<GameSession>();

                gameList.Add(session);

                await sessionList.AddOrUpdateAsync(tx, "all_games", gameList, (a, b) => gameList);
                await tx.CommitAsync();
            }

            return session;
        }
    }
}
