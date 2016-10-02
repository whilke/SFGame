using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using StructureMap.Pipeline;
using System.Fabric;
using Microsoft.ServiceFabric.Data;

namespace enBask.Core.GameManager
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var container = ContainerFactory.Get<SFRegistry>();
                //new GameManager(context)
                ServiceRuntime.RegisterServiceAsync("GameManagerType",
                    context =>
                    {

                        IReliableStateManagerReplica stateMgr = new ReliableStateManager(context, null);
                        container.Inject(stateMgr);

                        var args = new ExplicitArguments();
                        args.Set(context);

                        var gameService = container
                            .GetInstance<GameManager>(args);

                        return gameService;
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(GameManager).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
