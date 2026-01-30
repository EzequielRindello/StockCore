using StockCore.Services.Const;
using System.ComponentModel.DataAnnotations;

public class ProductForm
{
    public int? Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.NameRequired)]
    [MaxLength(150, ErrorMessage = ValidationMessages.NameMax150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.DescriptionRequired)]
    [MaxLength(500, ErrorMessage = ValidationMessages.DescriptionMax500)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.SkuRequired)]
    [MaxLength(50, ErrorMessage = ValidationMessages.SkuMax50)]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.CategoryRequired)]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
