using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;

namespace TimedWebScrap;

public static class Blob
{
    [FunctionName("Blob")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger("get")] HttpRequest req)
    {
        string blobName = req.Query["blob"];
        var client = new BlobContainerClient("UseDevelopmentStorage=true", "fromtempfiletxt");
        var blobClient = client.GetBlobClient(blobName);
        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        
        return blobName != null
            ? (ActionResult)new OkObjectResult($"{content}")
            : new BadRequestObjectResult("Please pass a blob on the query string");
    }
}