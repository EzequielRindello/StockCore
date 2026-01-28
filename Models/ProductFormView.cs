using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductFormView
{
    public ProductForm Product { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
}
