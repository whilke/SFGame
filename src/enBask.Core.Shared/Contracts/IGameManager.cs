using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.Shared.Contracts
{
    public interface IGameManager : IService
    {
        Task<GameSession> Create(string partitionId);
        Task<GameSession> Find(string gameId);
        Task<bool> Destroy(GameSession session);
        Task<List<GameSession>> Query();
    }
}
