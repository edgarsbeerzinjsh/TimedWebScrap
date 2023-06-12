using AzureUtils;
using Microsoft.WindowsAzure.Storage;

namespace TimedScrapAPI
{
    public class StorageAccountFactory : IStorageAccountFactory
    {
        private readonly IAzureStorageConfiguration _config;

        public StorageAccountFactory(IAzureStorageConfiguration config)
        {
            _config = config;
        }

        public CloudStorageAccount CreateCloudStorageAccount()
        {
            return CloudStorageAccount.Parse(_config.AzureStorageAccount);
        }
    }
}
