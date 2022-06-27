using ExtensionsPlatform.Application.MgyGateway.Dto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.Application.MgyGateway
{
    public interface IMgyGateWay
    {
        Task<string> GetMgyCompanyEndpoint(string networkId, string extId);
        //Task<string> GetMgyCompanyEndpointLocal(string networkId);
    }


    public class MgyGateWay : IMgyGateWay
    {
        private readonly HttpClient _client;
        private readonly IOptions<ApplicationSettings> _settings;

        public MgyGateWay(HttpClient client, IOptions<ApplicationSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        private static ConcurrentDictionary<string, string> _gatewayPathByNetworkId = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> _gatewayPathByNetworkIdLocal = new ConcurrentDictionary<string, string>();

        public async Task<string> GetMgyCompanyEndpoint(string networkId,string extId)
        {
            string connection;
            if (_gatewayPathByNetworkId.TryGetValue(networkId, out connection))
            {
                return connection;
            }

            var mgyGatewayEndpoint = $"{_settings.Value.MgyGatewayEndpoint}{extId}";
            var targetUri = string.Format(mgyGatewayEndpoint, networkId);
            
            var response = await _client.GetAsync(targetUri).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
                ExtensionEndPoint extensionEndPoint = JsonConvert.DeserializeObject<ExtensionEndPoint>(responseStream);
                _gatewayPathByNetworkId.TryAdd(networkId, extensionEndPoint?.Connection);
                return extensionEndPoint?.Connection;
            }

            return null;
        }

        public async Task<string> GetMgyCompanyEndpointLocal(string networkId)
        {
            string connection;
            if (_gatewayPathByNetworkIdLocal.TryGetValue(networkId, out connection))
            {
                return connection;
            }

            var mgyGatewayEndpoint = _settings.Value.MgyGatewayEndpoint;
            var targetUri = string.Format(mgyGatewayEndpoint, networkId);
            // TODO: CACHE this
            var response = await _client.GetAsync(targetUri).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                ExtensionEndPoint extensionEndPoint = JsonConvert.DeserializeObject<ExtensionEndPoint>(responseStream);
                _gatewayPathByNetworkIdLocal.TryAdd(networkId, extensionEndPoint?.Local);
                return extensionEndPoint?.Local;
            }

            return null;
        }
    }
}
