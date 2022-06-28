using ExtensionsPlatform.DataStorageClient;
using ExtensionsPlatform.Repo.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.Repo
{
    public interface IExtensionCompanyCosmoRepo
    {
        Task<CompanyEntity> AddNewCompanyDataAsync(CompanyEntity entity);
        Task UpdateCompanyDataAsync(CompanyEntity entity);
        Task<IEnumerable<CompanyEntity>> GetCompaniesDataBy(string networkId);
        Task<CompanyEntity> GetCompanyDataBy(string networkId, string extensionName);
        Task DeleteCompanyData(CompanyEntity company);

        Task<IEnumerable<string>> GetCompaniesPartitionKey();
    }


    public class ExtensionCompanyCosmoRepo : IExtensionCompanyCosmoRepo
    {
        private readonly ICosmosDBStorageClient _dataStorageClient;
        private readonly Container _container;
        private readonly string DB_Company_CONTAINER = "Companies";
        private readonly ILogger<ExtensionCompanyCosmoRepo> _logger;

        public ExtensionCompanyCosmoRepo(ICosmosDBStorageClient dataStorageClient,
            ILogger<ExtensionCompanyCosmoRepo> logger)
        {
            _dataStorageClient = dataStorageClient;
            _container = _dataStorageClient.GetCosmosContainer(DB_Company_CONTAINER).Result;
            _logger = logger;
        }

        public async Task<CompanyEntity> AddNewCompanyDataAsync(CompanyEntity entity)
        {
            var result = await _dataStorageClient.AddItemAsync(_container, entity);
            if (result == null)
            {
                return default;
            }
            return result.Resource;
        }

        public Task UpdateCompanyDataAsync(CompanyEntity entity)
        {
            return _dataStorageClient.UpsertItemAsync(_container, entity);
        }

        public Task<IEnumerable<CompanyEntity>> GetCompaniesDataBy(string networkId)
        {
            return _dataStorageClient.GetItemsBy<CompanyEntity>(_container, inv => inv.NetworkId == networkId);
        }

        public Task<IEnumerable<string>> GetCompaniesPartitionKey()
        {
            return _dataStorageClient.GetItemsPK<CompanyEntity>(_container, e => e.PartitionKey);
        }

        public Task<CompanyEntity> GetCompanyDataBy(string networkId, string extensionName)
        {
            return _dataStorageClient.GetItemBy<CompanyEntity>(_container, inv => inv.NetworkId == networkId && inv.ExtensionName == extensionName);
        }


        public Task DeleteCompanyData(CompanyEntity company)
        {
            return _dataStorageClient.DeleteItemAsync<CompanyEntity>(_container, company.CompanyId, company);
        }
    }
}
