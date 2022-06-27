using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExtensionsPlatform.Application.ExtensionCompany.Dto;
using ExtensionsPlatform.Application.ExtensionCompany;
using ExtensionsPlatform.Application.Exception;
using ExtensionsPlatform.Triggers.Validators;

namespace ExtensionsPlatform.Triggers.Http
{
    public class UpdateExtensionVersionHttpTrigger
    {
        private readonly ICompanyService _companyService;
        private readonly IRequestModelValidator _modelValidator;

        public UpdateExtensionVersionHttpTrigger(ICompanyService companyService,
            IRequestModelValidator modelValidator)
        {
            _companyService = companyService;
            _modelValidator = modelValidator;
        }

        [FunctionName("UpdateExtensionVersion")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var entry = JsonConvert.DeserializeObject<ExtCompanyVersionInfo>(requestBody);
                var modelErrors = _modelValidator.ValidateRequestModel(entry);
                if (!string.IsNullOrEmpty(modelErrors))
                {
                    return new BadRequestObjectResult(modelErrors);
                }

                await _companyService.UpdateExtensionVersion(entry);

                return new OkObjectResult("The extension version was Updated");
            }
            catch (BaseException ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception exc)
            {
                log.LogError(exc.Message);
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}
