using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Entities;
using StockCore.Services.Interfaces;

namespace StockCore.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductList>> GetAllAsync()
        {
            return await _context.Products
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

        public async Task<ProductDetail> GetDetailAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockMovements)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new Exception("Product not found");

            var stock = product.StockMovements.Sum(m =>
                m.MovementType == Dtos.Enums.StockMovementType.In
                    ? m.Quantity
                    : -m.Quantity);

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

        public async Task<ProductForm> GetForEditAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

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

        public async Task CreateAsync(ProductForm model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Sku = model.Sku,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductForm model)
        {
            var product = await _context.Products.FindAsync(model.Id);

            if (product == null)
                throw new Exception("Product not found");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Sku = model.Sku;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteManyAsync(List<int> ids)
        {
            var products = _context.Products.Where(p => ids.Contains(p.Id));
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
        }
    }
}
