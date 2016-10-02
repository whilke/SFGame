using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using enBask.Core.Shared.Contracts;
using enBask.Core.Shared;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using enBask.Core.GameManager.interfaces;
using Microsoft.ServiceFabric.Data;

namespace enBask.Core.GameManager
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class GameManager : StatefulService, IGameManager
    {
        ILifetimeManager _lifetimeMgr;
        public GameManager(StatefulServiceContext context, ILifetimeManager lifeManager, IReliableStateManagerReplica stateMgr)
            : base(context, stateMgr)
        {
            _lifetimeMgr = lifeManager;
        }

        #region IGameManager
        public async Task<GameSession> Create(string partitionId)
        {
            return await _lifetimeMgr.CreateAsync("MafiaGame", partitionId);
        }

        public Task<bool> Destroy(GameSession session)
        {
            throw new NotImplementedException();
        }

        public async Task<GameSession> Find(string gameId)
        {
            return new GameSession();
        }

        public async Task<List<GameSession>> Query()
        {
            List<GameSession> sessions = new List<GameSession>();
            return sessions;
        }
        #endregion

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
          {
                new ServiceReplicaListener(
                    context => new FabricTransportServiceRemotingListener(context, this))
            };
        }


    }
}
