public interface IHomeService
{
    Task<object> GetDashboardDataAsync();
    Task<string> ExportDashboardReportAsync();
}
