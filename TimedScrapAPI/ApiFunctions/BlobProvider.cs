using System.Net;
using System.Text;
using System.Threading.Tasks;
using AzureServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace TimedScrapAPI.ApiFunctions
{
    public class BlobProvider
    {
        private readonly ILogger<BlobProvider> _logger;
        private readonly IAzureTableService _tableService;
        private readonly IAzureBlobService _blobService;

        public BlobProvider(ILogger<BlobProvider> log, IAzureTableService tableService, IAzureBlobService blobService)
        {
            _logger = log;
            _tableService = tableService;
            _blobService = blobService;
        }

        [FunctionName("BlobProvider")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "BlobKey" })]
        [OpenApiParameter(name: "logId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Blob** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string logId = req.Query["logId"];

            var logEntry = await _tableService.GetLogEntry(logId);

            var content = await _blobService.GetBlobContent(logEntry.RowKey);

            return new FileContentResult(Encoding.UTF8.GetBytes(content.ToCharArray()), "application/json");

        }
    }
}
