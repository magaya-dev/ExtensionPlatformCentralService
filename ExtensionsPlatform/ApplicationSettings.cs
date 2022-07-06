using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform
{
    public class ApplicationSettings
    {
        public string MgyGatewayEndpoint { get; set; }

        public string CosmosDbConnection { get; set; }

        public string CosmosDbName { get; set; }

        public int TimerMinutes { get; set; }

        //public string AzureWebJobsStorage { get; set; }
    }
}
