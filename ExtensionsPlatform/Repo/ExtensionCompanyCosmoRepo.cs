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
        Task<ExtensionCompanyEntity> AddNewCompanyDataAsync(ExtensionCompanyEntity entity);
        Task UpdateCompanyDataAsync(ExtensionCompanyEntity entity);
        Task<IEnumerable<ExtensionCompanyEntity>> GetCompaniesDataBy(string networkId);
        Task<ExtensionCompanyEntity> GetCompanyDataBy(string networkId, string extensionName);
        Task DeleteCompanyData(ExtensionCompanyEntity company);

        Task<IEnumerable<string>> GetCompaniesPartitionKey();
    }


    public class ExtensionCompanyCosmoRepo : IExtensionCompanyCosmoRepo
    {
        private readonly ICosmosDBStorageClient _dataStorageClient;
        private readonly Container _container;
        private readonly string DB_Company_CONTAINER = "ExtensionCompany";
        private readonly ILogger<ExtensionCompanyCosmoRepo> _logger;

        public ExtensionCompanyCosmoRepo(ICosmosDBStorageClient dataStorageClient,
            ILogger<ExtensionCompanyCosmoRepo> logger)
        {
            _dataStorageClient = dataStorageClient;
            _container = _dataStorageClient.GetCosmosContainer(DB_Company_CONTAINER).Result;
            _logger = logger;
        }

        public async Task<ExtensionCompanyEntity> AddNewCompanyDataAsync(ExtensionCompanyEntity entity)
        {
            var result = await _dataStorageClient.AddItemAsync(_container, entity);
            if (result == null)
            {
                return default;
            }
            return result.Resource;
        }

        public Task UpdateCompanyDataAsync(ExtensionCompanyEntity entity)
        {
            return _dataStorageClient.UpsertItemAsync(_container, entity);
        }

        public Task<IEnumerable<ExtensionCompanyEntity>> GetCompaniesDataBy(string networkId)
        {
            return _dataStorageClient.GetItemsBy<ExtensionCompanyEntity>(_container, inv => inv.NetworkId == networkId);
        }

        public Task<IEnumerable<string>> GetCompaniesPartitionKey()
        {
            return _dataStorageClient.GetItemsPK<ExtensionCompanyEntity>(_container, e => e.PartitionKey);
        }

        public Task<ExtensionCompanyEntity> GetCompanyDataBy(string networkId, string extensionName)
        {
            return _dataStorageClient.GetItemBy<ExtensionCompanyEntity>(_container, inv => inv.NetworkId == networkId && inv.ExtensionName == extensionName);
        }


        public Task DeleteCompanyData(ExtensionCompanyEntity company)
        {
            return _dataStorageClient.DeleteItemAsync<ExtensionCompanyEntity>(_container, company.CompanyId, company);
        }
    }
}
