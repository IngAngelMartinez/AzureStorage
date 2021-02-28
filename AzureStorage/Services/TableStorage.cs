using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Productos.ExtensionsMethods;
using Productos.Models;
using AzureStorage.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace AzureStorage.Services
{
    //https://docs.microsoft.com/en-us/rest/api/storageservices/Query-Operators-Supported-for-the-Table-Service?redirectedfrom=MSDN
    //https://github.com/Azure-Samples/azure-cosmos-table-dotnet-core-getting-started/blob/main/CosmosTableSamples/AdvancedSamples.cs

    public class TableStorage
    {
        private readonly string connectionStringStorage;
        private readonly ILogger logger;
        
        public TableStorage(IConfiguration configuration, ILogger<TableStorage> logger)
        {
            connectionStringStorage = configuration.GetSection("ConnectionStrings:AzureStorage").Value;
            this.logger = logger;
        }

        public async Task<Response<T>> Get<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity
        {
            try
            {

                CloudTable table = await CloudTable(tableName);
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);

                T entry = (T)result.Result;

                if (entry != null)
                {
                    return new Response<T>(entry);
                }
                else
                {
                    return new Response<T>("Not Found");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"PartitionKey: {partitionKey} {Environment.NewLine}RowKey: {rowKey}");
                return new Response<T>(ex.Message);
            }
        }

        public async Task<Response<List<T>>> GetAll<T>(string tableName) where T : TableEntity, new()
        {
            List<T> entryList = new List<T>();

            CloudTable table = await CloudTable(tableName);
            TableQuery<T> query = new TableQuery<T>();
                
            foreach (var entry in table.ExecuteQuery(query))
            {
                entryList.Add(entry);
            }

            return new Response<List<T>>(entryList);
        }

        public async Task<Response<List<T>>> GetFilter<T>(string tableName, Expression<Func<T, bool>> predicate, int take = 50) where T : TableEntity, new()
        {
                       
            CloudTable table = await CloudTable(tableName);
            var query = table.CreateQuery<T>().Where(predicate).Take(take);

            return new Response<List<T>>(query.ToList());

        }

        public async Task<Response<T>> AddOrMerge<T>(T entry, string tableName, string partitionKey) where T : TableEntity
        {
            CloudTable table = await CloudTable(tableName);

            entry.PartitionKey = partitionKey;
            entry.RowKey = DateTime.Now.Ticks.ToString();

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entry);
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

            T insert = (T)result.Result;

            return new Response<T>(insert);

        }

        public async Task<Response<int>> Delete<T>(T entry, string tableName) where T : TableEntity
        {
            CloudTable table = await CloudTable(tableName);
            TableOperation deleteOperation = TableOperation.Delete(entry);
            TableResult result = await table.ExecuteAsync(deleteOperation);

            return new Response<int>(result.HttpStatusCode);
        }


        private async Task<CloudTable> CloudTable(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionStringStorage);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable cloudTable = tableClient.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }




        public async Task<Response<BD_Product>> AddALot()
        {

            try
            {
                CloudTable table = await CloudTable("Demo");

                BD_Product entry = new BD_Product();

                for (int i = 0; i < 500; i++)
                {
                    entry.PartitionKey = "5";
                    entry.RowKey = DateTime.Now.Ticks.ToString();
                    entry.Description = i.ToString();

                    TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entry);
                    TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                }
                

                return new Response<BD_Product>(new BD_Product());

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
                return new Response<BD_Product>(ex.Message);
            }
        }

    }
}
