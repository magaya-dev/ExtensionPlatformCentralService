
using ExtensionsPlatform.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Repo.Data
{
    public class ExtensionActiveEntity : IEntity
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string PartitionKey => NetworkId;

        public string NetworkId { get; set; }
        public string ExtensionName { get; set; }
        public bool Active { get; set; }
    }

}
