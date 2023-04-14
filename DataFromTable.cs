using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;

namespace TimedWebScrap;

public static class DataFromTable
{
    [FunctionName("DataFromTable")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger("get")] HttpRequest req)
    {
        string fromParam = req.Query["start"];
        string toParam = req.Query["end"];
        fromParam ??= "2000-01-01";
        toParam ??= "2100-01-01";
        DateTime from = DateTime.Parse(fromParam);
        DateTime to = DateTime.Parse(toParam);
        
        TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "webscrap");
        var products = tableClient.Query<Product>(x => x.Timestamp > from & x.Timestamp < to);
        var answer = products.Aggregate("", (current, item) => current + $"{item.PartitionKey}, {item.RowKey}\n");

        return new OkObjectResult($"{answer}");
    }
}