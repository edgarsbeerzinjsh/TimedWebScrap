using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task WriteInTable(bool success, DateTime date)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);
            await table.CreateIfNotExistsAsync();

            var entity = new ScrapEntity(Guid.NewGuid().ToString())
            {
                ScrapExacutedAt = date,
                Success = success
            };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }
    }
}

