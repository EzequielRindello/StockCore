using Microsoft.EntityFrameworkCore;
using StockCore.Entities;

namespace StockCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<ApplicationUserEntity> Users { get; set; }
    }
}
