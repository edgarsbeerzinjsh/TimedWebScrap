using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using AzureUtils;

namespace AzureServices
{
    public class AzureTableService : IAzureTableService
    {
        private readonly IAzureStorageConfiguration _config;
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public AzureTableService(IAzureStorageConfiguration config, IStorageAccountFactory factory)
        {
            _config = config;
            _storageAccount = factory.CreateCloudStorageAccount();
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(_config.TableName);
        }

        public async Task<Guid> WriteInTable(bool success)
        {
            await _table.CreateIfNotExistsAsync();
            var id = Guid.NewGuid();
            var entity = new ScrapEntity(id.ToString())
            {
                ScrapExacutedAt = DateTime.UtcNow,
                Success = success
            };

            var insertOperation = TableOperation.Insert(entity);

            await _table.ExecuteAsync(insertOperation);

            return id;
        }

        public async Task<List<ScrapEntity>> ListOfLogRecords(DateTime from, DateTime to)
        {
            await _table.CreateIfNotExistsAsync();

            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterConditionForDate("ScrapExacutedAt", QueryComparisons.GreaterThanOrEqual, from),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForDate("ScrapExacutedAt", QueryComparisons.LessThanOrEqual, to));

            var query = new TableQuery<ScrapEntity>().Where(filter);

            return (await _table.ExecuteQuerySegmentedAsync(query, null)).Results;
        }

        public async Task<ScrapEntity> GetLogEntry(string logId)
        {
            await _table.CreateIfNotExistsAsync();
            var query = new TableQuery<ScrapEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, logId));

            return (await _table.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }
    }
}

