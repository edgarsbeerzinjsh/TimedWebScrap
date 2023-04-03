using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace TimedWebScrap
{
    // C# record type for items in the table
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
            [TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            var nameContent = DateTime.Now;
            var idNameByDate = $"{nameContent.Year}_{nameContent.Month}_{nameContent.Day}_{nameContent.Hour}_{nameContent.Minute}_{nameContent.Second}";
            // Create a request for the URL.
            //HttpClient HttpWReq = new HttpClient();
                    //.("https://api.publicapis.org/random?auth=null");
            HttpWebRequest HttpWReq =
                (HttpWebRequest)WebRequest.Create("https://api.publicapis.org/random?auth=null");

            // Get the response.
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            //Status
            var status = HttpWResp.StatusDescription;
            Console.WriteLine(status);
            
            // Get the stream containing content returned by the server.
            Stream dataStream = HttpWResp.GetResponseStream();
            
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new(dataStream);

            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            HttpWResp.Close();

            // Display the content.
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}\n" +
                $"{responseFromServer}\n");
            
            // New instance of TableClient class referencing the server-side table
            TableClient tableClient = new TableClient(
                "UseDevelopmentStorage=true", "webscrap"
            );

            await tableClient.CreateIfNotExistsAsync();
            
            // Create new item using composite key constructor
            var scrap = new Product()
            {
                PartitionKey = idNameByDate,
                RowKey = status
            };

            // Add new item to server-side table
            await tableClient.AddEntityAsync(scrap);
            
            
            //Blob to local storage
            var client = new BlobContainerClient(
                "UseDevelopmentStorage=true", "fromtempfiletxt"
            );
            await client.CreateIfNotExistsAsync();
            
            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "data";
            Directory.CreateDirectory(localPath);
            string fileName = idNameByDate + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);
            
            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, responseFromServer);
            
            // Get a reference to a blob
            BlobClient blobClient = client.GetBlobClient(fileName);

            // Upload data from the local file
            await blobClient.UploadAsync(localFilePath, true);
            //BlobClient blobClient = client.GetBlobClient(responseFromServer);
            //await client.UploadBlobAsync(idNameByDate, blobClient);


        }
    }
}
