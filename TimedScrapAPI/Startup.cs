using AzureServices;
using AzureUtils;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

[assembly: FunctionsStartup(typeof(TimedScrapAPI.Startup))]

namespace TimedScrapAPI
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAzureStorageConfiguration, AzureStorageConfiguration>();
            builder.Services.AddScoped<IPublicRandomAPI>(provider => 
                RestService.For<IPublicRandomAPI>(Environment.GetEnvironmentVariable("PublicAPIUrl")));
            builder.Services.AddSingleton<IStorageAccountFactory, StorageAccountFactory>();
            builder.Services.AddSingleton<IAzureTableService, AzureTableService>();
            builder.Services.AddSingleton<IAzureBlobService, AzureBlobService>();
        }
    }
}
