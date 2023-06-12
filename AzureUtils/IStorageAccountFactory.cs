using Microsoft.WindowsAzure.Storage;

namespace AzureUtils
{
    public interface IStorageAccountFactory
    {
        CloudStorageAccount CreateCloudStorageAccount();
    }
}
