using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.GameManager
{
    public static class ServiceDescriptorFactory
    {
        private static Dictionary<string, ServiceDescriptor> _services;
        static ServiceDescriptorFactory()
        {
            _services = new Dictionary<string, ServiceDescriptor>();
            _services["MafiaGame"] = new ServiceDescriptor
            {
                AppUri = new Uri("fabric:/enBask.Core.GameApp"),
                ServiceName = "MafiaGame",
                ServiceType = "MafiaGameServiceType",
                MinReplicaCount = 2,
                TargetReplicaCount = 3
            };
        }


        public static ServiceDescriptor Get(string name)
        {
            ServiceDescriptor sd;
            _services.TryGetValue(name, out sd);
            return sd;
        }
    }

    public class ServiceDescriptor
    {
        public string ServiceType { get; set; }
        public string ServiceName { get; set; }
        public Uri AppUri { get; set; }
        public int MinReplicaCount { get; set; }
        public int TargetReplicaCount { get; set; }

        public Uri GenerateServiceUri(string gameId)
        {
            var baseUri = AppUri.OriginalString;
            var serviceUri = new Uri(string.Format("{0}/{1}_{2}", baseUri, ServiceName, gameId));
            return serviceUri;
        }
    }
}
