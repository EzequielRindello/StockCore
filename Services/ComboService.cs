using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockCore.Data;
public class ComboService : IComboService
{
    private readonly ApplicationDbContext _db;

    public ComboService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SelectListItem>> GetCategoriesCombo()
    {
        var categories = await _db
            .Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        var result = new List<SelectListItem>();

        foreach (var category in categories)
        {
            result.Add(new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name
            });
        }

        return result;
    }
}
