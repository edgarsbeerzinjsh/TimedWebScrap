using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;

namespace TimedWebScrap
{
    public record Product : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
    }
    public class WebScrap
    {
        [FunctionName("WebScrap")]
        public async Task Run(
            [TimerTrigger("0 * * * * *")]TimerInfo myTimer)
        {
            var nameContent = DateTime.UtcNow;
            var idNameByDate = $"{nameContent.Year}_{nameContent.Month}_{nameContent.Day}_{nameContent.Hour}_{nameContent.Minute}.txt";

            var HttpWReq = (HttpWebRequest)WebRequest.Create("https://api.publicapis.org/random?auth=null");
            var HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
            var status = HttpWResp.StatusDescription;
            var dataStream = HttpWResp.GetResponseStream();
            StreamReader reader = new(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            HttpWResp.Close();

            var tableClient = new TableClient("UseDevelopmentStorage=true", "webscrap");
            await tableClient.CreateIfNotExistsAsync();
            var scrap = new Product()
            {
                PartitionKey = idNameByDate,
                RowKey = status
            };
            await tableClient.AddEntityAsync(scrap);
            
            var client = new BlobContainerClient("UseDevelopmentStorage=true", "fromtempfiletxt");
            await client.CreateIfNotExistsAsync();
            var localPath = "data";
            Directory.CreateDirectory(localPath);
            var localFilePath = Path.Combine(localPath, idNameByDate);
            await File.WriteAllTextAsync(localFilePath, responseFromServer);
            var blobClient = client.GetBlobClient(idNameByDate);
            await blobClient.UploadAsync(localFilePath, true);
        }
    }
}
