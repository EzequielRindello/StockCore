using Microsoft.AspNetCore.Mvc.Rendering;

public interface IComboService
{
    Task<List<SelectListItem>> GetCategoriesCombo();
    Task<List<SelectListItem>> GetProductsCombo();
}