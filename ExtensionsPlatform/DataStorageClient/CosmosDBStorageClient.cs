using ExtensionsPlatform.DataModel;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPlatform.DataStorageClient
{
    public interface ICosmosDBStorageClient
    {
        Task<Container> GetCosmosContainer(string containerId, string partitionKeyName = "/PartitionKey");
        Task<ItemResponse<T>> AddItemAsync<T>(Container container, T item) where T : IEntity;
        Task<T> DeleteItemAsync<T>(Container container, string id, string partitionKey = null) where T : IEntity;
        Task<T> DeleteItemAsync<T>(Container container, string id, T item) where T : IEntity;
        Task<T> GetItemAsync<T>(Container container, string id) where T : IEntity;
        Task<TResult> GetItemBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection);
        Task<T> GetItemBy<T>(Container container, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection);
        Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, TResult>> projection);
        IAsyncEnumerable<IEnumerable<T>> GetItemsBy<T>(Container container);
        Task<IEnumerable<T>> GetItemsBy<T>(Container container, Expression<Func<T, bool>> predicate);
        Task ReplaceItemAsync<T>(Container container, string id, T item) where T : IEntity;
        Task UpsertItemAsync<T>(Container container, T item) where T : IEntity;
        Task<ItemResponse<T>> UpdateItemAsync<T>(Container container, T item) where T : IEntity;
    }

    public class CosmosDBStorageClient : ICosmosDBStorageClient
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;

        public CosmosDBStorageClient(IOptions<ApplicationSettings> settings)
        {
            _cosmosClient = new CosmosClient(settings.Value.CosmosDbConnection);

            _database = CreateDatabaseAsync(settings.Value.CosmosDbName).Result;
        }

        private Task<DatabaseResponse> CreateDatabaseAsync(string database)
        {
            return _cosmosClient.CreateDatabaseIfNotExistsAsync(database);
        }


        public async Task<Container> GetCosmosContainer(string containerId, string partitionKeyName = "/PartitionKey")
        {
            return await _database.CreateContainerIfNotExistsAsync(containerId, partitionKeyName);
        }

        public async Task<ItemResponse<T>> AddItemAsync<T>(Container container, T item) where T : IEntity
        {
            try
            {
                return await container.CreateItemAsync(item, new PartitionKey(item.PartitionKey)).ConfigureAwait(false);
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return default;
                // Log container item already exists
            }
        }

        public async Task<T> GetItemAsync<T>(Container container, string id) where T : IEntity
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false);
                return response.Resource;
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // ItemResponse<T> response1 = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                // Log container item already exists
                throw exc;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public Task<T> GetItemBy<T>(Container container, Expression<Func<T, bool>> predicate) => GetItemBy(container, predicate, t => t);

        public async Task<TResult> GetItemBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection)
        {
            var iterator = container.GetItemLinqQueryable<T>()
                                .Where(predicate)
                                .Select(projection)
                                .ToFeedIterator();

            if (!iterator.HasMoreResults)
            {
                return default;
            }

            var response = await iterator.ReadNextAsync().ConfigureAwait(false);
            return response.FirstOrDefault();
        }

        // Add projection pending
        public async IAsyncEnumerable<IEnumerable<T>> GetItemsBy<T>(Container container)
        {
            var iterator = container.GetItemLinqQueryable<T>().ToFeedIterator();

            if (!iterator.HasMoreResults)
            {
                yield return default;
            }

            while (iterator.HasMoreResults)
            {
                var items = new List<T>();
                FeedResponse<T> currentResultSet = await iterator.ReadNextAsync().ConfigureAwait(false);
                foreach (T item in currentResultSet)
                {
                    items.Add(item);
                }

                yield return items;
            }
        }

        public Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, TResult>> projection) => GetItemsBy(container, null, projection);

        public Task<IEnumerable<T>> GetItemsBy<T>(Container container, Expression<Func<T, bool>> predicate) => GetItemsBy(container, predicate, t => t);

        public async Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection)
        {
            var items = new List<TResult>();
            FeedIterator<TResult> feedIterator;
            var query = container.GetItemLinqQueryable<T>();
            if (predicate != null)
            {
                feedIterator = query.Where(predicate).Select(projection).ToFeedIterator();
            }
            else
            {
                feedIterator = query.Select(projection).ToFeedIterator();
            }

            //Asynchronous query execution
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<TResult> currentResultSet = await feedIterator.ReadNextAsync().ConfigureAwait(false);
                foreach (TResult item in currentResultSet)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<T> DeleteItemAsync<T>(Container container, string id, T item) where T : IEntity
        {
            try
            {
                var partKey = new PartitionKey(item.PartitionKey);
                var result = await container.DeleteItemAsync<T>(id, partKey).ConfigureAwait(false);
                return result.Resource;
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw exc;
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                // throw new StaleObjectException() Handle outside and retry
                throw exc;
            }
        }

        public async Task<T> DeleteItemAsync<T>(Container container, string id, string partitionKey = null) where T : IEntity
        {
            try
            {
                var partKey = new PartitionKey(partitionKey ?? id);
                var result = await container.DeleteItemAsync<T>(id, partKey).ConfigureAwait(false);
                return result.Resource;
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw exc; // capture  and log
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                // throw new StaleObjectException() Handle outside and retry
                throw exc; // capture and log
            }
        }

        public async Task ReplaceItemAsync<T>(Container container, string id, T item) where T : IEntity
        {
            try
            {
                var partKey = new PartitionKey(item.PartitionKey);
                await container.ReplaceItemAsync(item, id, partKey).ConfigureAwait(false);
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(item, new PartitionKey(item.PartitionKey));
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                // throw new StaleObjectException() Handle outside and retry
                throw exc;
            }
        }

        public async Task UpsertItemAsync<T>(Container container, T item) where T : IEntity
        {
            try
            {
                var partKey = new PartitionKey(item.PartitionKey);
                await container.UpsertItemAsync(item, partKey, new ItemRequestOptions { IfMatchEtag = item.ETag }).ConfigureAwait(false);
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                // throw new StaleObjectException() Handle outside and retry
                throw exc;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<ItemResponse<T>> UpdateItemAsync<T>(Container container, T item) where T : IEntity
        {
            try
            {
                var partKey = new PartitionKey(item.PartitionKey);
                return await container.UpsertItemAsync(item, partKey, new ItemRequestOptions { IfMatchEtag = item.ETag }).ConfigureAwait(false);
            }
            catch (CosmosException exc) when (exc.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                // throw new StaleObjectException() Handle outside and retry
                throw exc;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
