public interface ICategoryService
{
    Task<List<CategoryList>> GetAllCategories();
    Task<List<CategoryList>> FilterCategories(CategoryFilter filter);
    Task<CategoryDetail> GetDetail(int id);
    Task<CategoryForm> GetForEdit(int id);
    Task<(string, string)> CreateCategory(CategoryForm model);
    Task<(CategoryForm, string, string)> UpdateCategory(CategoryForm model);
    Task<(string, string)> DeleteManyCategories(List<int> ids);
    Task<string> ExportCategoriesCsvAsync(CategoryFilter filter)
}