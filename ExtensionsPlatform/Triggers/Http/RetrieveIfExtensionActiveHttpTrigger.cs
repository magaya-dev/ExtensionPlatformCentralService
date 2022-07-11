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
    public class RetrieveIfExtensionActiveHttpTrigger
    {
        private readonly IActivarExtensionService _activarExtService;
        private readonly IRequestModelValidator _modelValidator;

        public RetrieveIfExtensionActiveHttpTrigger(IActivarExtensionService activarExtService,
            IRequestModelValidator modelValidator)
        {
            _activarExtService = activarExtService;
            _modelValidator = modelValidator;
        }

        [FunctionName("RetrieveIfExtensionActive")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# ActivateExtension HTTP trigger function processed a request.");

            try
            {
                string networkId = req.Query["networkId"];
                string extName = req.Query[""];
                if (string.IsNullOrEmpty(networkId) || string.IsNullOrEmpty(extName))
                {
                    return new BadRequestObjectResult("Must enter the NetworkId and the ExtName in the request parameters");
                }

                return new OkObjectResult(await _activarExtService.RetrieveNetworkIdExtensionActive(networkId, extName));
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
