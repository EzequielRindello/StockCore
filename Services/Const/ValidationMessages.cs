namespace StockCore.Services.Const
{
    public static class ValidationMessages
    {
        // Consts
        public const string ERROR = "Error";
        public const string SUCCESS = "Success";


        // General messages
        public static string NotFound(string entity)
        {
            return $"{entity} not found";
        }

        public static string CreatedMessage(string entity)
        {
            return $"{entity} created successfully";
        }

        public static string SavedMessage(string entity)
        {
            return $"{entity} saved successfully";
        }

        public static string DeletedMessage(string entity)
        {
            return $"{entity} deleted successfully";
        }

        public static string ErrorMessage(string action, string entity)
        {
            return $"Error {action} {entity}";
        }

        public static string SelectedMessage(string entity)
        {
            return $"No {entity} selected";
        }

        // Categories
        public static string CategoryWithProducts()
        {
            return "Categories that have associated products cannot be deleted.";
        }

        // Stock 
        public static string ProductWithStock()
        {
            return "Products with associated stock cannot be deleted.";
        }
    }
}
