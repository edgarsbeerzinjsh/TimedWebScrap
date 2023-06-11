using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Refit;

namespace TimedScrapAPI
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var service = RestService.For<IPublicRandomAPI>("https://api.publicapis.org");

            var data = await service.GetApi();

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("randomapidata");
            await table.CreateIfNotExistsAsync();

            var date = DateTime.UtcNow;
            var entity = new ScrapEntity(Guid.NewGuid().ToString())
            {
                ScrapExacutedAt = date,
                Success = data.IsSuccessStatusCode
            };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("randomapiblobs");
            await container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference($"{date.Date.ToString("yyyy_MM_dd_T_HH_mm_ss")}.json");
            //var byteContent = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data.Content).ToCharArray());
            //using (var stream = new MemoryStream(byteContent))

            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(data.Content));
            
            //await table.CreateIfNotExistsAsync();

            //var date = DateTime.UtcNow;
            //var entity = new ScrapEntity(Guid.NewGuid().ToString())
            //{
            //    ScrapExacutedAt = date,
            //    Success = data.IsSuccessStatusCode
            //};

            //var insertOperation = TableOperation.Insert(entity);

            //await table.ExecuteAsync(insertOperation);

            log.LogInformation(JsonConvert.SerializeObject(data.Content));
        }
    }
}
