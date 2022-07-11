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
    public interface IActiveExtensionCosmoRepo
    {
        Task<ExtensionActiveEntity> AddNewExtensionActiveDataAsync(ExtensionActiveEntity entity);
        Task UpdateExtensionActiveDataAsync(ExtensionActiveEntity entity);
        Task<IEnumerable<ExtensionActiveEntity>> GetExtensionsActiveDataBy(string networkId);
        Task<ExtensionActiveEntity> GetExtensionActiveDataBy(string networkId, string extensionName);
        Task DeleteExtensionActiveData(ExtensionActiveEntity company);
    }


    public class ActiveExtensionCosmoRepo : IActiveExtensionCosmoRepo
    {
        private readonly ICosmosDBStorageClient _dataStorageClient;
        private readonly Container _container;
        private readonly string DB_Company_CONTAINER = "ExtensionActive";
        private readonly ILogger<ActiveExtensionCosmoRepo> _logger;

        public ActiveExtensionCosmoRepo(ICosmosDBStorageClient dataStorageClient,
            ILogger<ActiveExtensionCosmoRepo> logger)
        {
            _dataStorageClient = dataStorageClient;
            _container = _dataStorageClient.GetCosmosContainer(DB_Company_CONTAINER).Result;
            _logger = logger;
        }

        public async Task<ExtensionActiveEntity> AddNewExtensionActiveDataAsync(ExtensionActiveEntity entity)
        {
            var result = await _dataStorageClient.AddItemAsync(_container, entity);
            if (result == null)
            {
                return default;
            }
            return result.Resource;
        }

        public Task UpdateExtensionActiveDataAsync(ExtensionActiveEntity entity)
        {
            return _dataStorageClient.UpsertItemAsync(_container, entity);
        }

        public Task<IEnumerable<ExtensionActiveEntity>> GetExtensionsActiveDataBy(string networkId)
        {
            return _dataStorageClient.GetItemsBy<ExtensionActiveEntity>(_container, inv => inv.NetworkId == networkId);
        }

        public Task<ExtensionActiveEntity> GetExtensionActiveDataBy(string networkId, string extensionName)
        {
            return _dataStorageClient.GetItemBy<ExtensionActiveEntity>(_container, inv => inv.NetworkId == networkId && inv.ExtensionName == extensionName);
        }


        public Task DeleteExtensionActiveData(ExtensionActiveEntity company)
        {
            return _dataStorageClient.DeleteItemAsync<ExtensionActiveEntity>(_container, company.Id, company);
        }
    }
}
