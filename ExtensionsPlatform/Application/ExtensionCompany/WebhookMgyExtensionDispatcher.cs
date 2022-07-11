using ExtensionsPlatform.Application.ExtensionCompany.Dto;
using ExtensionsPlatform.Application.MgyGateway;
using ExtensionsPlatform.Repo.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.Application.ExtensionCompany
{
    public interface IWebhookMgyExtensionDispatcher
    {
        Task Dispatch(ExtensionCompanyEntity extCompany);
    }


    public class WebhookMgyExtensionDispatcher : IWebhookMgyExtensionDispatcher
    {
        private readonly HttpClient _client;
        private readonly ILogger<WebhookMgyExtensionDispatcher> _logger;
        private readonly IMgyGateWay _mgyGateWay;
        private readonly ICompanyService _companyService;

        public WebhookMgyExtensionDispatcher(HttpClient client,
            ILogger<WebhookMgyExtensionDispatcher> logger,
            IMgyGateWay mgyGateWay,
            ICompanyService companyService)
        {
            _client = client;
            _logger = logger;
            _mgyGateWay = mgyGateWay;
            _companyService = companyService;
        }

        // Dispatch - Reques Magaya EndPOint
        public async Task Dispatch(ExtensionCompanyEntity extCompany)
        {
            try
            {
                var endpoint = await _mgyGateWay.GetMgyCompanyEndpoint(extCompany.NetworkId, extCompany.ExtId).ConfigureAwait(false);
                if (endpoint == null)
                {
                    if (!string.IsNullOrEmpty(extCompany.NetworkId))
                    {
                        _logger.LogError("Can't connect Magaya Gateway", extCompany.NetworkId);
                    }
                }
                else
                {
                    try
                    {
                        //var msgContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
                        //var createResponse = await _client.PostAsync($"{endpoint}/events/notificationsevents", msgContent).ConfigureAwait(false);

                        var response = new ExtCompanyVersionInfo 
                        { 
                            NetworkId = extCompany.NetworkId,
                            ExtensionName = extCompany.NetworkId,
                            CurrentVersion = "v.1.11.4",
                            UpdateAvailable = true
                        };

                        // Update entity
                        extCompany.Version = response.CurrentVersion;
                        extCompany.Latest = response.UpdateAvailable;
                        await _companyService.UpdateExtensionCompanyData(extCompany);
                    }
                    catch (System.Exception exc)
                    {
                        _logger.LogError($"Can't post event to Magaya {exc.Message}");
                    }
                }
            }
            catch (System.Exception exc)
            {
                _logger.LogError("Can't post event to Magaya", exc.Message);
            }
        }
    }
}
