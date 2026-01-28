namespace StockCore.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductList>> GetAllProducts();
        Task<ProductDetail> GetDetail(int id);
        Task<ProductForm> GetForEdit(int id);
        Task<(string key, string message)> CreateProduct(ProductForm model);
        Task<(ProductForm model, string key, string message)> UpdateProduct(ProductForm model);
        Task<(string key, string message)> DeleteManyProducts(List<int> ids);
    }
}
