using System;
using System.Threading.Tasks;
using ExtensionsPlatform.Application.ExtensionCompany;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExtensionsPlatform.Triggers.Timer
{
    public class UpdateExtCompanyVersionTimerTrigger
    {
        private readonly ICompanyService _companyService;
        private readonly IWebhookMgyExtensionDispatcher _webhookMgyExtensionDispatcher;

        public UpdateExtCompanyVersionTimerTrigger(ICompanyService companyService, IWebhookMgyExtensionDispatcher webhookMgyExtensionDispatcher)
        {
            _companyService = companyService;
            _webhookMgyExtensionDispatcher = webhookMgyExtensionDispatcher;
        }

        [FunctionName("UpdateExtCompanyVersionTimerTrigger")]
        public async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            try
            {
                var networkIds = await _companyService.GetNetworkIdList();

                log.LogInformation($"NetworkId List: {JsonConvert.SerializeObject(networkIds)}");

                foreach (var item in networkIds)
                {
                    try
                    {
                        log.LogInformation($"---Begin Iteraction for NID: {item}");
                        var extCompanyList = await _companyService.GetAllExtensionByNetworkId(item);
                        if (extCompanyList != null)
                        {
                            foreach (var extItem in extCompanyList)
                            {
                                //Validate UpdatedDate
                                if (DateTime.Now.Subtract(extItem.UpdatedDate.Value).TotalMinutes > 12)
                                {
                                    if (extItem.Status != Repo.Data.StatusExtension.Off_Line)
                                    {
                                        extItem.Status = Repo.Data.StatusExtension.Off_Line;
                                        await _companyService.UpdateExtensionCompanyData(extItem);
                                    }
                                    
                                } else
                                {
                                    if (extItem.Status != Repo.Data.StatusExtension.Online)
                                    {
                                        extItem.Status = Repo.Data.StatusExtension.Online;
                                        await _companyService.UpdateExtensionCompanyData(extItem);
                                    }   
                                }
                            }

                            //await _webhookMgyExtensionDispatcher.Dispatch(extCompany);
                        }
                        log.LogInformation($"---End Iteraction for NID: {item}");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                
            }
            catch (Exception exc)
            {
                log.LogError($"Error in UpdateExtCompanyVersion TimerTrigger: {exc.Message}");
                throw;
            }
        }
    }
}
