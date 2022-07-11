
using ExtensionsPlatform.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Repo.Data
{
    public class ExtensionCompanyEntity : IEntity
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty("id")]
        public string CompanyId { get; set; }

        public string PartitionKey => NetworkId;

        public string NetworkId { get; set; }

        public string CompanyName { get; set; }

        public string ExtensionName { get; set; }
        public string ExtId { get; set; }
        public string Version { get; set; }
        public string MgyVersion { get; set; }
        public bool Latest { get; set; }
        public StatusExtension Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool MgyGateWay { get; set; }
    }

    public enum StatusExtension
    {
        Online,
        Off_Line
    }
}
