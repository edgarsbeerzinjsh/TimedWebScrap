using AzureUtils;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureServices
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _container;

        public AzureBlobService(IAzureStorageConfiguration config, IStorageAccountFactory factory)
        {
            _client = factory.CreateCloudStorageAccount().CreateCloudBlobClient();
            _container = _client.GetContainerReference(config.BlobStorage);
        }
        public async Task UploadBlobContent(string content, string fileName)
        {
            await _container.CreateIfNotExistsAsync();

            var blockBlob = _container.GetBlockBlobReference($"{fileName}.json");

            await blockBlob.UploadTextAsync(content);
        }

        public async Task<string> GetBlobContent(string fileName)
        {
            await _container.CreateIfNotExistsAsync();

            var blockBlob = _container.GetBlockBlobReference($"{fileName}.json");

            return await blockBlob.DownloadTextAsync();
        }
    }
}
