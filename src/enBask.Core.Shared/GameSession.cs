using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enBask.Core.Shared
{
    public class GameSession
    {
        public string PartitionKey { get; set; }
        public string GameId { get; set; }
    }
}
