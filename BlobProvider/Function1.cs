using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace BlobProvider
{
    public static class Function1
    {
        [FunctionName("BlobProvider")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string logId = req.Query["logId"];

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var azureTableService = new AzureTableService("randomapidata", storageAccount);

            var logEntry = await azureTableService.GetLogEntry(logId);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("randomapiblobs");
            await container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference($"{logEntry.RowKey}.json");

            var content = await blockBlob.DownloadTextAsync();

            return new FileContentResult(Encoding.UTF8.GetBytes(content.ToCharArray()), "application/json");

        }
    }
}
