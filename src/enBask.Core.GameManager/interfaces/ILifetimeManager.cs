using enBask.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.GameManager.interfaces
{
    public interface ILifetimeManager
    {
        Task<GameSession> CreateAsync(string serviceType, string pKey);
    }
}
