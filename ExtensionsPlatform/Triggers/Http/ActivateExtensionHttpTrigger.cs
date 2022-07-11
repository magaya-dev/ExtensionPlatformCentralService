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
using ExtensionsPlatform.Application.ExtensionActiveAction.Dto;
using ExtensionsPlatform.Application.ExtensionActiveAction;

namespace ExtensionsPlatform.Triggers.Http
{
    public class ActivateExtensionHttpTrigger
    {
        private readonly IActivarExtensionService _activarExtService;
        private readonly IRequestModelValidator _modelValidator;

        public ActivateExtensionHttpTrigger(IActivarExtensionService activarExtService,
            IRequestModelValidator modelValidator)
        {
            _activarExtService = activarExtService;
            _modelValidator = modelValidator;
        }

        [FunctionName("ActivateExtension")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# ActivateExtension HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var entry = JsonConvert.DeserializeObject<ExtensionActive>(requestBody);
                var modelErrors = _modelValidator.ValidateRequestModel(entry);
                if (!string.IsNullOrEmpty(modelErrors))
                {
                    return new BadRequestObjectResult(modelErrors);
                }

                await _activarExtService.ActiveNetworkIdExtension(entry);

                return new OkObjectResult("Activate extension was processed");
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
