using ExtensionsPlatform.Application.Exception;
using ExtensionsPlatform.Application.ExtensionCompany;
using ExtensionsPlatform.Application.ExtensionCompany.Dto;
using ExtensionsPlatform.Application.MgyGateway;
using ExtensionsPlatform.Repo;
using ExtensionsPlatform.Repo.Data;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExtensionsPlatform.UnitTest.Application
{
    public class CompanyServiceTest
    {
        private readonly Mock<IExtensionCompanyCosmoRepo> _extRepo;
        private readonly Mock<IMgyGateWay>_mgyGateWay;

        private readonly CompanyService _companyService;

        public CompanyServiceTest()
        {
            _extRepo = new Mock<IExtensionCompanyCosmoRepo>();
            _mgyGateWay = new Mock<IMgyGateWay>();

            _companyService = new CompanyService(_extRepo.Object, _mgyGateWay.Object);
        }

        [Fact]
        public async Task CreateExtensionCompany_GetCompanyDataBy_ThrowExistsExtensionCompanyException()
        {
            var entry = new ExtensionCompanyData
            {
                NetworkId = "21458",
                CompanyName = "Company Name",
                ExtensionName = "Rate Managment",
                ExtId = "catapult"
            };

            var extCompanyData = new ExtensionCompanyEntity
            {
                CompanyId = Guid.NewGuid().ToString(),
                NetworkId = entry.NetworkId,
                ExtensionName = entry.ExtensionName,
                CompanyName = entry.CompanyName,
                ExtId = entry.ExtId,
                MgyVersion = null,
                Status = StatusExtension.Online,
                Latest = true,
                Version = "4.1.2",
                MgyGateWay = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            var user = _extRepo.Setup(arg => arg.GetCompanyDataBy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(extCompanyData);

            await Assert.ThrowsAsync<ExistsExtensionCompanyException>(async () => await _companyService.CreateExtensionCompany(entry));
        }

        [Fact]
        public async Task CreateExtensionCompany_AddNewCompanyDataAsync_ReturnRespOK()
        {
            var entry = new ExtensionCompanyData
            {
                NetworkId = "21458",
                CompanyName = "Company Name",
                ExtensionName = "Rate Managment",
                ExtId = "catapult"
            };

            var extCompanyData = new ExtensionCompanyEntity
            {
                CompanyId = Guid.NewGuid().ToString(),
                NetworkId = entry.NetworkId,
                ExtensionName = entry.ExtensionName,
                CompanyName = entry.CompanyName,
                ExtId = entry.ExtId,
                MgyVersion = null,
                Status = StatusExtension.Online,
                Latest = true,
                Version = "4.1.2",
                MgyGateWay = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            var user = _extRepo.Setup(arg => arg.GetCompanyDataBy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((ExtensionCompanyEntity)null);

            var response = await _companyService.CreateExtensionCompany(entry);

            _extRepo.Verify(repo => repo.AddNewCompanyDataAsync(It.Is<ExtensionCompanyEntity>(e => 
            e.NetworkId == entry.NetworkId &&
            e.CompanyName == entry.CompanyName &&
            e.ExtensionName == entry.ExtensionName &&
            e.ExtId == entry.ExtId)));

            Assert.Equal("OK", response);
        }

        [Fact]
        public async Task UpdateExtensionVersion_GetCompanyDataBy_ThrowNotExistsExtensionCompanyException()
        {
            var entry = new ExtCompanyVersionInfo
            {
                NetworkId = "21458",
                ExtensionName = "Rate Managment",
                CurrentVersion = "2.58",
                UpdateAvailable = true
            };

            var user = _extRepo.Setup(arg => arg.GetCompanyDataBy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((ExtensionCompanyEntity)null);

            await Assert.ThrowsAsync<NotExistsExtensionCompanyException>(async () => await _companyService.UpdateExtensionVersion(entry));
        }

        [Fact]
        public async Task UpdateExtensionVersion_UpdateCompanyDataAsync_responseOK()
        {
            var entry = new ExtCompanyVersionInfo
            {
                NetworkId = "21458",
                ExtensionName = "Rate Managment",
                CurrentVersion = "2.58",
                UpdateAvailable = true
            };

            var extCompanyData = new ExtensionCompanyEntity
            {
                CompanyId = Guid.NewGuid().ToString(),
                NetworkId = entry.NetworkId,
                ExtensionName = entry.ExtensionName,
                CompanyName = "Name",
                ExtId = "catapult",
                MgyVersion = null,
                Status = StatusExtension.Online,
                Latest = true,
                Version = "4.1.2",
                MgyGateWay = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            var user = _extRepo.Setup(arg => arg.GetCompanyDataBy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(extCompanyData);

            await _companyService.UpdateExtensionVersion(entry);

            _extRepo.Verify(repo => repo.UpdateCompanyDataAsync(It.Is<ExtensionCompanyEntity>(e =>
            e.NetworkId == entry.NetworkId &&
            e.CompanyName == extCompanyData.CompanyName &&
            e.ExtensionName == entry.ExtensionName &&
            e.ExtId == extCompanyData.ExtId &&
            e.Version == entry.CurrentVersion &&
            e.Latest == entry.UpdateAvailable)));
        }

        [Fact]
        public async Task GetExtensionCompanyData_GetCompanyDataBy_responseOK()
        {
            var networkId = "36760";
            var extName = "Rate Managment";

            var extCompanyData = new ExtensionCompanyEntity
            {
                CompanyId = Guid.NewGuid().ToString(),
                NetworkId = networkId,
                ExtensionName = extName,
                CompanyName = "Name",
                ExtId = "catapult",
                MgyVersion = null,
                Status = StatusExtension.Online,
                Latest = true,
                Version = "4.1.2",
                MgyGateWay = true,
                CreatedDate = DateTime.Parse("6/28/2022 5:11:58 PM"),
                UpdatedDate = DateTime.Parse("6/28/2022 5:21:58 PM")
            };

            var data = _extRepo.Setup(arg => arg.GetCompanyDataBy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(extCompanyData);

            var resp = await _companyService.GetExtensionCompanyData(networkId, extName);

            Assert.Equal(JsonConvert.SerializeObject(resp), JsonConvert.SerializeObject(extCompanyData));
        }

        [Fact]
        public async Task GetAllExtensionByNetworkId_GetCompaniesDataBy_ThrowNotExistsExtensionCompanyException()
        {
            var networkId = "21458";

            var data = _extRepo.Setup(arg => arg.GetCompaniesDataBy(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<ExtensionCompanyEntity>)null);

            await Assert.ThrowsAsync<NotExistsExtensionCompanyException>(async () => await _companyService.GetAllExtensionByNetworkId(networkId));
        }

        [Fact]
        public async Task GetAllExtensionByNetworkId_GetCompaniesDataBy_responseListExt()
        {
            var networkId = "36760";

            var extCompanyDataList = new ExtensionCompanyEntity[]
            {
                new ExtensionCompanyEntity 
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    NetworkId = networkId,
                    ExtensionName = "Ext Name",
                    CompanyName = "Name",
                    ExtId = "catapult",
                    MgyVersion = null,
                    Status = StatusExtension.Online,
                    Latest = true,
                    Version = "4.1.2",
                    MgyGateWay = true,
                    CreatedDate = DateTime.Parse("6/28/2022 5:11:58 PM"),
                    UpdatedDate = DateTime.Parse("6/28/2022 5:21:58 PM")
                }
            };

            var list = _extRepo.Setup(arg => arg.GetCompaniesDataBy(It.IsAny<string>()))
                .ReturnsAsync(extCompanyDataList);

            var resp = await _companyService.GetAllExtensionByNetworkId(networkId);

            Assert.Equal(JsonConvert.SerializeObject(resp), JsonConvert.SerializeObject(extCompanyDataList));
        }

        [Fact]
        public async Task UpdateExtensionCompanyData_UpdateCompanyDataAsync_responseOK()
        {
            var networkId = "36760";
            var extName = "Rate Managment";

            var extCompanyData = new ExtensionCompanyEntity
            {
                CompanyId = Guid.NewGuid().ToString(),
                NetworkId = networkId,
                ExtensionName = extName,
                CompanyName = "Name",
                ExtId = "catapult",
                MgyVersion = null,
                Status = StatusExtension.Online,
                Latest = true,
                Version = "4.1.2",
                MgyGateWay = true,
                CreatedDate = DateTime.Parse("6/28/2022 5:11:58 PM"),
                UpdatedDate = DateTime.Parse("6/28/2022 5:21:58 PM")
            };

            _extRepo.Setup(arg => arg.UpdateCompanyDataAsync(It.IsAny<ExtensionCompanyEntity>())).Verifiable();

            await _companyService.UpdateExtensionCompanyData(extCompanyData);

            _extRepo.Verify(repo => repo.UpdateCompanyDataAsync(It.Is<ExtensionCompanyEntity>(e =>
            e.ExtId == extCompanyData.ExtId &&
            e.Version == extCompanyData.Version &&
            e.Latest == extCompanyData.Latest)));
        }

        [Fact]
        public async Task GetNetworkIdList_GetCompaniesPartitionKey_responseListClientsOK()
        {

            var NIdList = new string[] { "36760", "33288" };

            var data = _extRepo.Setup(arg => arg.GetCompaniesPartitionKey())
                .ReturnsAsync(NIdList);

            var resp = await _companyService.GetNetworkIdList();

            Assert.Equal(JsonConvert.SerializeObject(resp), JsonConvert.SerializeObject(NIdList));

        }
    }
}
