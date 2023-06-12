namespace AzureUtils
{
    public interface IAzureStorageConfiguration
    {
        string TableName { get; }
        string AzureStorageAccount { get; }
        string BlobStorage { get; }
    }
}
