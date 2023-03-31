using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TimedWebScrap
{
    public class WebScrap
    {
        [FunctionName("WebScrap")]
        public async Task Run(
            [TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
          
            // Create a request for the URL.
            HttpWebRequest HttpWReq =
                (HttpWebRequest)WebRequest.Create("https://api.publicapis.org/random?auth=null");

            // Get the response.
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            //Status
            Console.WriteLine(HttpWResp.StatusDescription);
            
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
            
            //Blob to local storage
            var client = new BlobContainerClient(
                "UseDevelopmentStorage=true", "test"
            );
            await client.CreateIfNotExistsAsync();
            
            await client.UploadBlobAsync(responseFromServer, dataStream);
        }
    }
}
