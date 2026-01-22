using StockCore.Entities;

namespace StockCore.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any())
                return;

            // Categories
            var categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Name = "Office Supplies", Description = "Office and stationery products" },
                new Category { Name = "Cleaning", Description = "Cleaning and hygiene products" },
                new Category { Name = "Furniture", Description = "Office and warehouse furniture" },
                new Category { Name = "IT Equipment", Description = "IT infrastructure and hardware" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Products
            var products = new List<Product>
            {
                new Product { Name = "Lenovo ThinkPad X1", Sku = "EL-001", CategoryId = categories[0].Id },
                new Product { Name = "Dell 27 Monitor", Sku = "EL-002", CategoryId = categories[0].Id },
                new Product { Name = "Logitech MX Mouse", Sku = "EL-003", CategoryId = categories[0].Id },
                new Product { Name = "HP Laser Printer", Sku = "IT-001", CategoryId = categories[4].Id },
                new Product { Name = "Cisco Router", Sku = "IT-002", CategoryId = categories[4].Id },
                new Product { Name = "A4 Paper Box", Sku = "OF-001", CategoryId = categories[1].Id },
                new Product { Name = "Stapler", Sku = "OF-002", CategoryId = categories[1].Id },
                new Product { Name = "Office Chair", Sku = "FR-001", CategoryId = categories[3].Id },
                new Product { Name = "Standing Desk", Sku = "FR-002", CategoryId = categories[3].Id },
                new Product { Name = "Disinfectant Spray", Sku = "CL-001", CategoryId = categories[2].Id }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Stock Movements
            var stockMovements = new List<StockMovement>
            {
                new StockMovement { ProductId = products[0].Id, Quantity = 10, MovementType = Dtos.Enums.StockMovementType.In },
                new StockMovement { ProductId = products[1].Id, Quantity = 5, MovementType = Dtos.Enums.StockMovementType.In },
                new StockMovement { ProductId = products[2].Id, Quantity = 20, MovementType = Dtos.Enums.StockMovementType.In },
                new StockMovement { ProductId = products[0].Id, Quantity = 2, MovementType = Dtos.Enums.StockMovementType.Out, Reason = "Employee assignment" },
                new StockMovement { ProductId = products[6].Id, Quantity = 50, MovementType = Dtos.Enums.StockMovementType.In }
            };

            context.StockMovements.AddRange(stockMovements);
            context.SaveChanges();

            // Suppliers
            var suppliers = new List<Supplier>
            {
                new Supplier { Name = "Tech Distributors Inc", ContactEmail = "sales@techdist.com", Phone = "+1-555-1000" },
                new Supplier { Name = "Office World", ContactEmail = "contact@officeworld.com", Phone = "+1-555-2000" },
                new Supplier { Name = "CleanPro Ltd", ContactEmail = "info@cleanpro.com", Phone = "+1-555-3000" }
            };

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            // Company
            var company = new Company
            {
                Name = "StockCore Solutions",
                Address = "742 Evergreen Terrace, Springfield",
                Email = "info@stockcore.com"
            };

            context.Company.Add(company);
            context.SaveChanges();

            // this is for the initial user, password is not hashed for the sake of simplicity
            // created users will have a hashed password :)
            var users = new List<ApplicationUserEntity>
            {
                new ApplicationUserEntity
                {
                    UserName = "admin",
                    Email = "test@test.com",
                    PasswordHash = "admin",
                    EmailConfirmed = true
                },
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
