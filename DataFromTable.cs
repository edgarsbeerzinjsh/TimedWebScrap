using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TimedWebScrap;

public static class DataFromTable
{
    [FunctionName("DataFromTable")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string fromParam = req.Query["start"];
        string toParam = req.Query["end"];

        if (fromParam == null)
        {
            fromParam = "2000-01-01";
        }
        if (toParam == null)
        {
            toParam = "2100-01-01";
        }
        // Parse the timestamps from the query parameters
        DateTime from = DateTime.Parse(fromParam);
        DateTime to = DateTime.Parse(toParam);


        //
        TableClient tableClient = new TableClient(
        "UseDevelopmentStorage=true", "webscrap");
        
        // Read multiple items from container
        var products = tableClient.Query<Product>(x => x.Timestamp > from & x.Timestamp < to);
        var answer = "";
        foreach (var item in products)
        {
            answer += $"{item.PartitionKey}, {item.RowKey}\n";
        }
        
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        fromParam = fromParam ?? data?.name;

        return fromParam != null
            ? (ActionResult)new OkObjectResult($"{answer}")
            : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        
    }
}