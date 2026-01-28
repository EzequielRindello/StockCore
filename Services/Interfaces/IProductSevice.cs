namespace StockCore.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductList>> GetAllAsync();
        Task<ProductDetail> GetDetailAsync(int id);
        Task<ProductForm> GetForEditAsync(int id);
        Task CreateAsync(ProductForm model);
        Task UpdateAsync(ProductForm model);
        Task DeleteAsync(int id);
        Task DeleteManyAsync(List<int> ids);
    }
}
