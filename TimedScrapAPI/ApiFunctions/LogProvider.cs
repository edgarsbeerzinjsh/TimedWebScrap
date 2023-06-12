using System;
using System.Net;
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
    public class LogProvider
    {
        private readonly ILogger<LogProvider> _logger;
        private readonly IAzureTableService _tableService;

        public LogProvider(ILogger<LogProvider> log, IAzureTableService tableService)
        {
            _logger = log;
            _tableService = tableService;
        }

        [FunctionName("LogProvider")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "LogList" })]
        [OpenApiParameter(name: "from", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **from** parameter")]
        [OpenApiParameter(name: "to", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **to** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            DateTime.TryParse(req.Query["from"], out var from);
            DateTime.TryParse(req.Query["to"], out var to);

            var data = await _tableService.ListOfLogRecords(from, to);

            return new OkObjectResult(data);
        }
    }
}
