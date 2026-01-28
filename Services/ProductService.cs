using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Dtos.Enums;
using StockCore.Entities;
using StockCore.Services.Const;
using StockCore.Services.Interfaces;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;

    public ProductService(ApplicationDbContext context)
    {
        _db = context;
    }

    public async Task<List<ProductList>> FilterProducts(ProductFilter filter)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(filter.Search) ||
                p.Sku.Contains(filter.Search));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filter.IsActive.Value);
        }

        return await query
            .Select(p => new ProductList
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Category = p.Category!.Name,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<ProductList>> GetAllProducts()
    {
        return await _db
            .Products
            .Include(p => p.Category)
            .Select(p => new ProductList
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Category = p.Category!.Name,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<ProductDetail> GetDetail(int id)
    {
        var product = await _db
            .Products
            .Include(p => p.Category)
            .Include(p => p.StockMovements)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            throw new Exception(ValidationMessages.NotFound("Product"));

        var stock = product.StockMovements.Sum(m =>
            m.MovementType == StockMovementType.In ? m.Quantity : -m.Quantity);

        return new ProductDetail
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Category = product.Category!.Name,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            Stock = stock
        };
    }

    public async Task<ProductForm> GetForEdit(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product == null)
            throw new Exception(ValidationMessages.NotFound("Product"));

        return new ProductForm
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive
        };
    }

    public async Task<(string, string)> CreateProduct(ProductForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Sku = model.Sku,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.CreatedMessage("Product"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("creating", "product"));
        }
    }

    public async Task<(ProductForm, string, string)> UpdateProduct(ProductForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var product = await _db.Products.FindAsync(model.Id);

            if (product == null)
                return (model, ValidationMessages.ERROR, ValidationMessages.NotFound("Product"));

            product.Name = model.Name;
            product.Description = model.Description;
            product.Sku = model.Sku;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (model, ValidationMessages.SUCCESS, ValidationMessages.SavedMessage("Product"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (model, ValidationMessages.ERROR, ValidationMessages.ErrorMessage("saving", "product"));
        }
    }

    public async Task<(string, string)> DeleteManyProducts(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return (ValidationMessages.ERROR, ValidationMessages.SelectedMessage("products"));

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var products = _db.Products.Where(p => ids.Contains(p.Id));
            _db.Products.RemoveRange(products);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return (ValidationMessages.SUCCESS, ValidationMessages.DeletedMessage("Product"));
        }
        catch
        {
            await tx.RollbackAsync();
            return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("deleting", "product"));
        }
    }
}
