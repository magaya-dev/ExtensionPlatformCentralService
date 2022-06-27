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
    public class CreateExtensionCompanyHttpTrigger
    {
        private readonly ICompanyService _companyService;
        private readonly IRequestModelValidator _modelValidator;

        public CreateExtensionCompanyHttpTrigger (ICompanyService companyService,
            IRequestModelValidator modelValidator)
        {
            _companyService = companyService;
            _modelValidator = modelValidator;
        }

        [FunctionName("CreateExtensionCompany")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var entry = JsonConvert.DeserializeObject<ExtensionCompanyData>(requestBody);
                var modelErrors = _modelValidator.ValidateRequestModel(entry);
                if (!string.IsNullOrEmpty(modelErrors))
                {
                    return new BadRequestObjectResult(modelErrors);
                }

                var response = await _companyService.CreateExtensionCompany(entry);

                return new OkObjectResult(response);
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
