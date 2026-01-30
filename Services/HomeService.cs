using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Dtos.Enums;
using StockCore.Models;

public class HomeService : IHomeService
{
    private readonly ApplicationDbContext _db;

    public HomeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDashboardDataAsync()
    {
        var now = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var sixMonthsAgo = now.AddMonths(-5);

        var summary = await GetDashboardSummaryAsync(firstDayOfMonth);

        var recentMovements = await _db.StockMovements
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .Select(s => new
            {
                s.Id,
                ProductName = s.Product!.Name,
                s.Quantity,
                s.MovementType,
                s.CreatedAt
            })
            .ToListAsync();

        var lowStockProducts = await _db.Products
            .AsNoTracking()
            .Select(p => new
            {
                p.Name,
                p.Sku,
                Stock = p.StockMovements.Sum(m =>
                    m.MovementType == StockMovementType.In ? m.Quantity : -m.Quantity)
            })
            .Where(p => p.Stock < 10)
            .OrderBy(p => p.Stock)
            .Take(5)
            .ToListAsync();

        var categoriesOverview = await _db.Categories
            .AsNoTracking()
            .OrderByDescending(c => c.Products.Count)
            .Take(5)
            .Select(c => new
            {
                c.Name,
                ProductCount = c.Products.Count()
            })
            .ToListAsync();

        var recentProducts = await _db.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new
            {
                p.Name,
                Category = p.Category!.Name,
                p.Sku,
                p.IsActive
            })
            .ToListAsync();

        var stockInOutChart = await _db.StockMovements
            .AsNoTracking()
            .Where(s => s.CreatedAt >= firstDayOfMonth)
            .GroupBy(s => s.MovementType)
            .Select(g => new
            {
                MovementType = g.Key,
                Total = g.Sum(x => x.Quantity)
            })
            .ToListAsync();

        var monthlyStockChart = await _db.StockMovements
            .AsNoTracking()
            .Where(s => s.CreatedAt >= sixMonthsAgo)
            .GroupBy(s => new
            {
                s.CreatedAt.Year,
                s.CreatedAt.Month,
                s.MovementType
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.MovementType,
                Total = g.Sum(x => x.Quantity)
            })
            .ToListAsync();

        var productsByCategoryChart = await _db.Categories
            .AsNoTracking()
            .Select(c => new
            {
                c.Name,
                ProductCount = c.Products.Count()
            })
            .ToListAsync();

        var stackedCategoryChart = await _db.Categories
            .AsNoTracking()
            .Select(c => new
            {
                Category = c.Name,
                StockIn = c.Products
                    .SelectMany(p => p.StockMovements)
                    .Where(m => m.MovementType == StockMovementType.In)
                    .Sum(m => m.Quantity),
                StockOut = c.Products
                    .SelectMany(p => p.StockMovements)
                    .Where(m => m.MovementType == StockMovementType.Out)
                    .Sum(m => m.Quantity)
            })
            .ToListAsync();

        return new
        {
            summary.TotalProducts,
            summary.TotalCategories,
            summary.StockInThisMonth,
            summary.StockOutThisMonth,
            recentMovements,
            lowStockProducts,
            categoriesOverview,
            recentProducts,
            stockInOutChart,
            productsByCategoryChart,
            monthlyStockChart,
            stackedCategoryChart
        };
    }

    public async Task<string> ExportDashboardReportAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var summary = await GetDashboardSummaryAsync(firstDayOfMonth);

        return
            "Metric,Value\n" +
            $"Total Products,{summary.TotalProducts}\n" +
            $"Total Categories,{summary.TotalCategories}\n" +
            $"Stock In (This Month),{summary.StockInThisMonth}\n" +
            $"Stock Out (This Month),{summary.StockOutThisMonth}\n";
    }

    private async Task<DashboardSummary> GetDashboardSummaryAsync(DateTime firstDayOfMonth)
    {
        return new DashboardSummary
        {
            TotalProducts = await _db.Products.CountAsync(),
            TotalCategories = await _db.Categories.CountAsync(),
            StockInThisMonth = await _db.StockMovements
                .Where(s => s.MovementType == StockMovementType.In && s.CreatedAt >= firstDayOfMonth)
                .SumAsync(s => s.Quantity),
            StockOutThisMonth = await _db.StockMovements
                .Where(s => s.MovementType == StockMovementType.Out && s.CreatedAt >= firstDayOfMonth)
                .SumAsync(s => s.Quantity)
        };
    }
}
