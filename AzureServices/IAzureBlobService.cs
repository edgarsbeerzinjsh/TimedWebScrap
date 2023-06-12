namespace AzureServices
{
    public interface IAzureBlobService
    {
        Task<string> GetBlobContent(string fileName);
        Task UploadBlobContent(string content, string fileName);
    }
}