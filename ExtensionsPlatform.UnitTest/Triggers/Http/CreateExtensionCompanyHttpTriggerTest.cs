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
    public class CreateExtensionCompanyHttpTriggerTest
    {
        private readonly Mock<ICompanyService> _companyService;
        private readonly Mock<IRequestModelValidator> _modelValidator;

        private readonly CreateExtensionCompanyHttpTrigger _createExtCompanyHttpTrigger;

        public CreateExtensionCompanyHttpTriggerTest()
        {
            _companyService = new Mock<ICompanyService>();
            _modelValidator = new Mock<IRequestModelValidator>();

            _createExtCompanyHttpTrigger = new CreateExtensionCompanyHttpTrigger(_companyService.Object, _modelValidator.Object);
        }

        [Fact]
        public async Task CreateExtensionCompany_ValidateRequestModelErrors_ThrowBadRequest()
        {
            var entry = new ExtensionCompanyData
            {
                NetworkId = "",
                CompanyName = "Company Name",
                ExtensionName = "Rate Managment",
                ExtId = "catapult"
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
            Assert.Equal(badRequest.StatusCode, ((BadRequestObjectResult)(await _createExtCompanyHttpTrigger.Run(request, logger))).StatusCode);
        }

        [Fact]
        public async Task CreateExtensionCompany_CreateExtensionCompanyApp_OkObjectResult()
        {
            var entry = new ExtensionCompanyData
            {
                NetworkId = "",
                CompanyName = "Company Name",
                ExtensionName = "Rate Managment",
                ExtId = "catapult"
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

            var response = "OK";
            _companyService.Setup(api => api.CreateExtensionCompany(It.IsAny<ExtensionCompanyData>()))
              .ReturnsAsync(response);

            var okResult = new OkObjectResult(response);
            Assert.Equal(okResult.StatusCode, ((OkObjectResult)(await _createExtCompanyHttpTrigger.Run(request, logger))).StatusCode);
        }
    }
}
