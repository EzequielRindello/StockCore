using StockCore.Dtos.Enums;
using System.ComponentModel.DataAnnotations;

public class StockMovementForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Movement type is required")]
    public StockMovementType MovementType { get; set; }

    [MaxLength(250)]
    public string? Reason { get; set; }
}