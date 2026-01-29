using System.ComponentModel.DataAnnotations;

public class CategoryForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}