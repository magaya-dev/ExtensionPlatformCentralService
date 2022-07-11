using ExtensionsPlatform.Application.Exception;
using ExtensionsPlatform.Application.ExtensionActiveAction.Dto;
using ExtensionsPlatform.Repo;
using ExtensionsPlatform.Repo.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.Application.ExtensionActiveAction
{
    public interface IActivarExtensionService
    {
        Task ActiveNetworkIdExtension(ExtensionActive entryData);
        Task<ExtensionActive> RetrieveNetworkIdExtensionActive(string networkId, string ExtName);
    }


    public class ActivarExtensionService : IActivarExtensionService
    {
        private readonly IActiveExtensionCosmoRepo _activeExtCosmoRepo;

        public ActivarExtensionService(IActiveExtensionCosmoRepo activeExtCosmoRepo)
        {
            _activeExtCosmoRepo = activeExtCosmoRepo;
        }

        public async Task ActiveNetworkIdExtension(ExtensionActive entryData)
        {
            var extData = await _activeExtCosmoRepo.GetExtensionActiveDataBy(entryData.NetworkId, entryData.ExtensionName);
            if (extData == null)
            {
                // ext does not exist - create
                var newExtData = await _activeExtCosmoRepo.AddNewExtensionActiveDataAsync(new ExtensionActiveEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    NetworkId = entryData.NetworkId,
                    ExtensionName = entryData.ExtensionName,
                    Active = entryData.Active
                });
            } else
            {
                // Update 
                extData.Active = entryData.Active;
                await _activeExtCosmoRepo.UpdateExtensionActiveDataAsync(extData);
            }
        }

        public async Task<ExtensionActive> RetrieveNetworkIdExtensionActive(string networkId, string ExtName)
        {
            var extData = await _activeExtCosmoRepo.GetExtensionActiveDataBy(networkId, ExtName);
            if (extData == null)
                throw new NotExistsExtensionActiveException();

            return new ExtensionActive
            {
                NetworkId = extData.NetworkId,
                ExtensionName = extData.ExtensionName,
                Active = extData.Active
            };
        }
    }
}
