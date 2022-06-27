using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ExtensionsPlatform.Application.MgyGateway.Dto
{
    public class ExtensionEndPoint
    {
        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("connection")]
        public string Connection { get; set; }

        [JsonPropertyName("local")]
        public string Local { get; set; }

        [JsonPropertyName("plus")]
        public bool Plus { get; set; }

        [JsonPropertyName("extensions")]
        public bool Extensions { get; set; }
    }
}
