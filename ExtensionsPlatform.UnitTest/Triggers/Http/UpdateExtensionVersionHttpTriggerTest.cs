using ExtensionsPlatform.Application.ExtensionCompany;
using ExtensionsPlatform.Application.ExtensionCompany.Dto;
using ExtensionsPlatform.Triggers.Http;
using ExtensionsPlatform.Triggers.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExtensionsPlatform.UnitTest.Triggers.Http
{
    public class UpdateExtensionVersionHttpTriggerTest
    {
        private readonly Mock<ICompanyService> _companyService;
        private readonly Mock<IRequestModelValidator> _modelValidator;

        private readonly UpdateExtensionVersionHttpTrigger _updateExtensionVersionHttpTrigger;

        public UpdateExtensionVersionHttpTriggerTest()
        {
            _companyService = new Mock<ICompanyService>();
            _modelValidator = new Mock<IRequestModelValidator>();

            _updateExtensionVersionHttpTrigger = new UpdateExtensionVersionHttpTrigger(_companyService.Object, _modelValidator.Object);
        }

        [Fact]
        public async Task UpdateExtensionVersion_ValidateRequestModelErrors_ThrowBadRequest()
        {
            var entry = new ExtCompanyVersionInfo
            {
                NetworkId = "21458",
                ExtensionName = "Rate Managment",
                CurrentVersion = "2.58",
                UpdateAvailable = true
            };

            var error = "NetworkId Required";
            _modelValidator.Setup(api => api.ValidateRequestModel(It.IsAny<object>()))
                .Returns(error);

            var context = new DefaultHttpContext();
            var request = context.Request;
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entry));
            MemoryStream stream = new MemoryStream(byteArray);
            request.Body = stream;

            Microsoft.Extensions.Logging.ILogger logger = NullLoggerFactory.Instance.CreateLogger("C# HTTP trigger function processed a request.");

            var badRequest = new BadRequestObjectResult(error);
            Assert.Equal(badRequest.StatusCode, ((BadRequestObjectResult)(await _updateExtensionVersionHttpTrigger.Run(request, logger))).StatusCode);
        }

        [Fact]
        public async Task CUpdateExtensionVersion_UpdateExtensionApp_OkObjectResult()
        {
            var entry = new ExtCompanyVersionInfo
            {
                NetworkId = "21458",
                ExtensionName = "Rate Managment",
                CurrentVersion = "2.58",
                UpdateAvailable = true
            };

            var error = "";
            _modelValidator.Setup(api => api.ValidateRequestModel(It.IsAny<object>()))
                .Returns(error);

            var context = new DefaultHttpContext();
            var request = context.Request;
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entry));
            MemoryStream stream = new MemoryStream(byteArray);
            request.Body = stream;

            Microsoft.Extensions.Logging.ILogger logger = NullLoggerFactory.Instance.CreateLogger("C# HTTP trigger function processed a request.");

            var response = "The extension version was Updated";
            _companyService.Setup(api => api.UpdateExtensionVersion(It.IsAny<ExtCompanyVersionInfo>())).Verifiable();

            var okResult = new OkObjectResult(response);
            Assert.Equal(okResult.StatusCode, ((OkObjectResult)(await _updateExtensionVersionHttpTrigger.Run(request, logger))).StatusCode);
        }
    }
}
