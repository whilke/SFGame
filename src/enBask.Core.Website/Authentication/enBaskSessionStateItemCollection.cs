using enBask.ASF.Tablestorage.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Xml.Serialization;

namespace enBask.Core.Website.Authentication
{
    public class enBaskSessionItem : BaseEntity
    {
        public static string GetPartition(string data)
        {
            return data.Substring(0, data.Length > 5 ? 5 : data.Length);
        }

        public enBaskSessionItem() { }
        public enBaskSessionItem(string key)
        {
            string partition = GetPartition(key);
            Id = key;
            Partition = partition;
        }

        public string StringData { get; set; }

        [JsonIgnore]
        public byte[] Data
        {
            get
            {
                return Convert.FromBase64String(StringData);
            }
            set
            {
                StringData = Convert.ToBase64String(value);
            }
        }

        public DateTime Expire { get; set; }
    }
}
