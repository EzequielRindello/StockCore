namespace StockCore.Models
{
    public class DashboardSummary
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int StockInThisMonth { get; set; }
        public int StockOutThisMonth { get; set; }
    }
}
