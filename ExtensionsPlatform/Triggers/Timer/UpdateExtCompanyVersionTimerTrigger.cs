using System;
using System.Threading.Tasks;
using ExtensionsPlatform.Application.ExtensionCompany;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ExtensionsPlatform.Triggers.Timer
{
    public class UpdateExtCompanyVersionTimerTrigger
    {
        private readonly ICompanyService _companyService;
        private readonly IOptions<ApplicationSettings> _settings;

        public UpdateExtCompanyVersionTimerTrigger(ICompanyService companyService, IOptions<ApplicationSettings> settings)
        {
            _companyService = companyService;
            _settings = settings;
        }

        [FunctionName("UpdateExtCompanyVersionTimerTrigger")]
        public async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            try
            {
                var networkIds = await _companyService.GetNetworkIdList();
                var minutesToUpdate = _settings.Value.TimerMinutes;

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
                                if (DateTime.Now.Subtract(extItem.UpdatedDate.Value).TotalMinutes > minutesToUpdate)
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
