using enBask.Core.GameManager.interfaces;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.GameManager
{
    public class SFRegistry : Registry
    {
        public SFRegistry()
        {
            ForSingletonOf<ILifetimeManager>().Use<LifetimeManager>();
            For<StatefulService>().Use<GameManager>();
        }
    }

    public static class ContainerFactory
    {
        public static Container Get<T>() where T: Registry, new()
        {
            var registry = new T();
            return new Container(registry);
        }
    }
}
