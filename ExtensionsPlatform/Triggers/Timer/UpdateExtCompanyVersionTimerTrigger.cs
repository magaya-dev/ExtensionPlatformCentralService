using System;
using System.Threading.Tasks;
using ExtensionsPlatform.Application.ExtensionCompany;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

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
                var networkIds = new string[] { "36760" };

                foreach (var item in networkIds)
                {

                }
                
                var extCompany = await _companyService.GetExtensionCompanyData("36760", "Acelynk Ext");
                if (extCompany != null)
                {
                    //Validate UpdatedDate
                    
                    //await _webhookMgyExtensionDispatcher.Dispatch(extCompany);
                }


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
