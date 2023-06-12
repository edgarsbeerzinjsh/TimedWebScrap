using System;
using System.Threading.Tasks;
using AzureServices;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TimedScrapAPI.ApiFunctions
{
    public class TimedScrap
    {
        private readonly IPublicRandomAPI _service;
        private readonly IAzureTableService _tableService;
        private readonly IAzureBlobService _blobService;
        public TimedScrap(IPublicRandomAPI service, IAzureTableService tableService, IAzureBlobService blobService)
        {
            _service = service;
            _tableService = tableService;
            _blobService = blobService;
        }

        [FunctionName("TimedScrap")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var data = await _service.GetApi();

            var id = await _tableService.WriteInTable(data.IsSuccessStatusCode);

            await _blobService.UploadBlobContent(JsonConvert.SerializeObject(data.Content), id.ToString());
        }
    }
}
