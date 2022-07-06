using ExtensionsPlatform.Application.MgyGateway;
using ExtensionsPlatform.Application.MgyGateway.Dto;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExtensionsPlatform.UnitTest.Application
{
    public class MgyGateWayTest
    {
        private readonly Mock<HttpMessageHandler> _client;
        private readonly Mock<IOptions<ApplicationSettings>> _settings;
        private static Mock<IConcurrentDictionary<string, string>> _gatewayPathByNetworkId;

        private readonly MgyGateWay _mgyGateWay;

        public MgyGateWayTest()
        {
            _client = new Mock<HttpMessageHandler>();
            var client = new HttpClient(_client.Object);
            _settings = new Mock<IOptions<ApplicationSettings>>();

            _mgyGateWay = new MgyGateWay(client, _settings.Object);
        }

        /*[Fact]
        public async Task MgyGateWay_NotFound_ReturnNull()
        {
            
            ApplicationSettings mgyGatewayEndpoint = new ApplicationSettings()
            {
                MgyGatewayEndpoint = "http://appgw.magaya.com:3042/connection/{0}?app=acelynk"
            };
            _settings.Setup(app => app.Value).Returns(mgyGatewayEndpoint);

            var endPointData = new ExtensionEndPoint
            {
                Company = "test",
                Connection = "http://appgw.magaya.com:3042/52321/ext",
                Local = "xx",
                Plus = true,
                Extensions = true
            };

            // To Get async GetMgyCompanyEndpoint
            _client.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Not Found", Encoding.UTF8, "application/json"),
                });

            var res = await _mgyGateWay.GetMgyCompanyEndpoint("36760", "catapult");

            Assert.Null(res);
        }*/

        [Fact]
        public async Task MgyGateWay_Success_RespondData()
        {
            string connection;
            _gatewayPathByNetworkId = new Mock<IConcurrentDictionary<string, string>>();

            _gatewayPathByNetworkId.Setup(g => g.TryGetValue(It.IsAny<string>(), out connection)).Returns(false);

            ApplicationSettings mgyGatewayEndpoint = new ApplicationSettings()
            {
                MgyGatewayEndpoint = "http://appgw.magaya.com:3042/connection/{0}?app=acelynk"
            };
            _settings.Setup(app => app.Value).Returns(mgyGatewayEndpoint);

            var endPointData = new ExtensionEndPoint
            {
                Company = "test",
                Connection = "http://appgw.magaya.com:3042/52321/ext",
                Local = "xx",
                Plus = true,
                Extensions = true
            };

            // To Get async GetMgyCompanyEndpoint
            _client.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(endPointData), Encoding.UTF8, "application/json"),
                });

            var expected = await _mgyGateWay.GetMgyCompanyEndpoint("36760", "catapult");

            Assert.Equal(expected, endPointData.Connection);
        }
    }

    public interface IConcurrentDictionary<TKey, TValue>
    {
        bool TryGetValue(string key, out string value);
    }
}
