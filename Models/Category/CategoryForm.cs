using StockCore.Services.Const;
using System.ComponentModel.DataAnnotations;

public class CategoryForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.NameRequired)]
    [MaxLength(100, ErrorMessage = ValidationMessages.NameMax100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.DescriptionRequired)]
    [MaxLength(250, ErrorMessage = ValidationMessages.DescriptionMax250)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
