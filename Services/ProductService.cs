using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Dtos.Enums;
using StockCore.Entities;
using StockCore.Services.Interfaces;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;

    public ProductService(ApplicationDbContext context)
    {
        _db = context;
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
            throw new Exception("Product not found");

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
            throw new Exception("Product not found");

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

            return ("Success", "Product created successfully");
        }
        catch
        {
            await tx.RollbackAsync();
            return ("Error", "Error creating product");
        }
    }

    public async Task<(string, string)> UpdateProduct(ProductForm model)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var product = await _db.Products.FindAsync(model.Id);

            if (product == null)
                return ("Error", "Product not found");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Sku = model.Sku;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return ("Success", "Product saved successfully");
        }
        catch
        {
            await tx.RollbackAsync();
            return ("Error", "Error saving product");
        }
    }

    public async Task<(string, string)> DeleteManyProducts(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return ("Error", "No products selected");

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var products = _db.Products.Where(p => ids.Contains(p.Id));
            _db.Products.RemoveRange(products);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return ("Success", "Products deleted successfully");
        }
        catch
        {
            await tx.RollbackAsync();
            return ("Error", "Error deleting products");
        }
    }
}
