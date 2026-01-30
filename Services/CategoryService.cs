using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Entities;
using StockCore.Services.Const;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;

    public CategoryService(ApplicationDbContext context)
    {
        _db = context;
    }

    public async Task<List<CategoryList>> FilterCategories(CategoryFilter filter)
    {
        var query = _db.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(c =>
                c.Name.Contains(filter.Search) ||
                (c.Description != null && c.Description.Contains(filter.Search)));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == filter.IsActive.Value);
        }

        return await query
            .Select(c => new CategoryList
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<List<CategoryList>> GetAllCategories()
    {
        return await _db
            .Categories
            .Select(c => new CategoryList
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryDetail> GetDetail(int id)
    {
        var category = await _db
            .Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            throw new Exception(ValidationMessages.NotFound("Category"));

        return new CategoryDetail
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            ProductCount = category.Products.Count
        };
    }

    public async Task<CategoryForm> GetForEdit(int id)
    {
        var category = await _db.Categories.FindAsync(id);

        if (category == null)
            throw new Exception(ValidationMessages.NotFound("Category"));

        return new CategoryForm
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task<(string, string)> CreateCategory(CategoryForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.CreatedMessage("Category"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("creating", "category"));
        }
    }

    public async Task<(CategoryForm, string, string)> UpdateCategory(CategoryForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var category = await _db.Categories.FindAsync(model.Id);

            if (category == null)
                return (model, ValidationMessages.ERROR, ValidationMessages.NotFound("Category"));

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (model, ValidationMessages.SUCCESS, ValidationMessages.SavedMessage("Category"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (model, ValidationMessages.ERROR, ValidationMessages.ErrorMessage("saving", "category"));
        }
    }

    public async Task<(string, string)> DeleteManyCategories(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return (ValidationMessages.ERROR, ValidationMessages.SelectedMessage("categories"));

        var hasProducts = await _db
            .Categories
            .AnyAsync(c => ids.Contains(c.Id) && c.Products.Any());

        if (hasProducts)
            return (ValidationMessages.ERROR, ValidationMessages.CategoryWithProducts());

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var categories = _db.Categories.Where(c => ids.Contains(c.Id));
            _db.Categories.RemoveRange(categories);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.DeletedMessage("Category"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("deleting", "category"));
        }
    }

}