using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Dtos.Enums;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        var now = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalProducts = await _db.Products.CountAsync();
        var totalCategories = await _db.Categories.CountAsync();

        var stockInThisMonth = await _db.StockMovements
            .Where(s => s.MovementType == StockMovementType.In && s.CreatedAt >= firstDayOfMonth)
            .SumAsync(s => s.Quantity);

        var stockOutThisMonth = await _db.StockMovements
            .Where(s => s.MovementType == StockMovementType.Out && s.CreatedAt >= firstDayOfMonth)
            .SumAsync(s => s.Quantity);

        var recentMovements = await _db.StockMovements
            .Include(s => s.Product)
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

        var productsWithStock = await _db.Products
            .Include(p => p.StockMovements)
            .Include(p => p.Category)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Sku,
                CategoryName = p.Category!.Name,
                p.IsActive,
                Stock = p.StockMovements.Sum(m =>
                    m.MovementType == StockMovementType.In ? m.Quantity : -m.Quantity)
            })
            .ToListAsync();

        var lowStockProducts = productsWithStock
            .Where(p => p.Stock < 10)
            .OrderBy(p => p.Stock)
            .Take(5)
            .ToList();

        var categoriesOverview = await _db.Categories
            .Include(c => c.Products)
            .OrderByDescending(c => c.Products.Count)
            .Take(5)
            .Select(c => new
            {
                c.Name,
                ProductCount = c.Products.Count
            })
            .ToListAsync();

        var recentProducts = await _db.Products
            .Include(p => p.Category)
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

        return Json(new
        {
            totalProducts,
            totalCategories,
            stockInThisMonth,
            stockOutThisMonth,
            recentMovements,
            lowStockProducts,
            categoriesOverview,
            recentProducts
        });
    }
}