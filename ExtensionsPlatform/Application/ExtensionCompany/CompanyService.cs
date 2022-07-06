using ExtensionsPlatform.Application.Exception;
using ExtensionsPlatform.Application.ExtensionCompany.Dto;
using ExtensionsPlatform.Application.MgyGateway;
using ExtensionsPlatform.Repo;
using ExtensionsPlatform.Repo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.Application.ExtensionCompany
{
    public interface ICompanyService
    {
        Task<string> CreateExtensionCompany(ExtensionCompanyData entrydata);
        Task<CompanyEntity> GetExtensionCompanyData(string networkId, string extName);
        Task UpdateExtensionCompanyData(CompanyEntity entrydata);
        Task UpdateExtensionVersion(ExtCompanyVersionInfo entrydata);
        Task<IEnumerable<string>> GetNetworkIdList();
        Task<IEnumerable<CompanyEntity>> GetAllExtensionByNetworkId(string networkId);
    }


    public class CompanyService : ICompanyService
    {
        private readonly IExtensionCompanyCosmoRepo _extensionCompanyCosmoRepo;
        private readonly IMgyGateWay _mgyGateWay;

        public CompanyService(IExtensionCompanyCosmoRepo extensionCompanyCosmoRepo,
            IMgyGateWay mgyGateWay)
        {
            _extensionCompanyCosmoRepo = extensionCompanyCosmoRepo;
            _mgyGateWay = mgyGateWay;
        }

        public async Task<string> CreateExtensionCompany(ExtensionCompanyData entrydata)
        {
            try
            {
                var extCompanyData = await _extensionCompanyCosmoRepo.GetCompanyDataBy(entrydata.NetworkId, entrydata.ExtensionName);

                if (extCompanyData != null)
                    throw new ExistsExtensionCompanyException();


                var companyCreated = await _extensionCompanyCosmoRepo.AddNewCompanyDataAsync(new CompanyEntity { 
                    CompanyId = Guid.NewGuid().ToString(),
                    NetworkId = entrydata.NetworkId,
                    CompanyName = entrydata.CompanyName,
                    ExtensionName = entrydata.ExtensionName,
                    ExtId = entrydata.ExtId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = StatusExtension.Online,
                    MgyGateWay =  string.IsNullOrEmpty(await _mgyGateWay.GetMgyCompanyEndpoint(entrydata.NetworkId, entrydata.ExtId)) ?
                        false : true
                } );

                return "OK";
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task UpdateExtensionVersion(ExtCompanyVersionInfo entrydata)
        {
            try
            {
                var extCompanyData = await _extensionCompanyCosmoRepo.GetCompanyDataBy(entrydata.NetworkId, entrydata.ExtensionName).ConfigureAwait(false);

                if (extCompanyData == null)
                    throw new NotExistsExtensionCompanyException();

                extCompanyData.Version = entrydata.CurrentVersion;
                extCompanyData.Latest = entrydata.UpdateAvailable;
                extCompanyData.UpdatedDate = DateTime.Now;
                extCompanyData.Status = StatusExtension.Online;

                await _extensionCompanyCosmoRepo.UpdateCompanyDataAsync(extCompanyData).ConfigureAwait(false);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<CompanyEntity> GetExtensionCompanyData(string networkId, string extName)
        {
            var extCompanyData = await _extensionCompanyCosmoRepo.GetCompanyDataBy(networkId, extName);

            return extCompanyData;
        }

        public async Task<IEnumerable<CompanyEntity>> GetAllExtensionByNetworkId(string networkId)
        {
            var extCompanyData = await _extensionCompanyCosmoRepo.GetCompaniesDataBy(networkId);

            if (extCompanyData == null || !extCompanyData.Any())
                throw new NotExistsExtensionCompanyException();

            return extCompanyData;
        }

        public async Task<IEnumerable<string>> GetNetworkIdList()
        {
            return await _extensionCompanyCosmoRepo.GetCompaniesPartitionKey();
        }

        public async Task UpdateExtensionCompanyData(CompanyEntity entrydata)
        {
            await _extensionCompanyCosmoRepo.UpdateCompanyDataAsync(entrydata);
        }


    }
}
