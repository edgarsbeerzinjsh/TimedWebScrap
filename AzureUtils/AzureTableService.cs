using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace AzureUtils
{
    public class AzureTableService
    {
        private readonly string _tableName;
        private readonly CloudStorageAccount _storageAccount;
        public AzureTableService(string tableName, CloudStorageAccount storageAccount)
        {
            _tableName = tableName;
            _storageAccount = storageAccount;

        }

        public async Task<Guid> WriteInTable(bool success)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();
            var id = Guid.NewGuid();
            var entity = new ScrapEntity(id.ToString())
            {
                ScrapExacutedAt = DateTime.UtcNow,
                Success = success
            };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);

            return id;
        }

        public async Task<List<ScrapEntity>> ListOfLogRecords(DateTime from, DateTime to)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();
            var query = new TableQuery<ScrapEntity>().Where(TableQuery.GenerateFilterConditionForDate("ScrapExacutedAt", QueryComparisons.LessThanOrEqual, to));
            return (await table.ExecuteQuerySegmentedAsync(query, null)).Results;
        }

        public async Task<ScrapEntity> GetLogEntry(string logId)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();
            var query = new TableQuery<ScrapEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, logId));
            
            return (await table.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }
    }
}

