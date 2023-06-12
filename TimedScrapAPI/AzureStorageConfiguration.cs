using AzureUtils;
using System;

namespace TimedScrapAPI
{
    public class AzureStorageConfiguration : IAzureStorageConfiguration
    {
        public string TableName { get; }
        public string AzureStorageAccount { get; }
        public string BlobStorage { get; }

        public AzureStorageConfiguration()
        {
            TableName = Environment.GetEnvironmentVariable("AzureTableName");
            AzureStorageAccount = Environment.GetEnvironmentVariable("AzureConnectionString");
            BlobStorage = Environment.GetEnvironmentVariable("AzureBlobContainer");
        }
    }
}
