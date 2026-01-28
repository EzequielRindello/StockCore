using System.ComponentModel.DataAnnotations;

public class ProductForm
{
    public int? Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string? Description { get; set; }

    [Required, MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
}
