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

        // Forms
        public const string NameRequired = "Name is required";
        public const string DescriptionRequired = "Description is required";
        public const string SkuRequired = "SKU is required";
        public const string CategoryRequired = "Category is required";
        public const string ProductRequired = "Product is required";
        public const string QuantityRequired = "Quantity is required";
        public const string MovementTypeRequired = "Movement type is required";
        public const string ReasonRequired = "Reason is required";
        public const string ActiveInactiveRequired = "Actitive/Inactive is required";
        public const string QuantityGreaterThanZero = "Quantity must be greater than 0";
        public const string NameMax100 = "Name must not exceed 100 characters";
        public const string NameMax150 = "Name must not exceed 150 characters";
        public const string DescriptionMax250 = "Description must not exceed 250 characters";
        public const string DescriptionMax500 = "Description must not exceed 500 characters";
        public const string SkuMax50 = "SKU must not exceed 50 characters";
    }
}
