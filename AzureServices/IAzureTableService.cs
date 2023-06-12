namespace AzureServices
{
    public interface IAzureTableService
    {
        Task<ScrapEntity> GetLogEntry(string logId);
        Task<List<ScrapEntity>> ListOfLogRecords(DateTime from, DateTime to);
        Task<Guid> WriteInTable(bool success);
    }
}