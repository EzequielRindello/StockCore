using StockCore.Dtos.Enums;
using StockCore.Services.Const;
using System.ComponentModel.DataAnnotations;

public class StockMovementForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.ProductRequired)]
    public int ProductId { get; set; }

    [Required(ErrorMessage = ValidationMessages.QuantityRequired)]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.QuantityGreaterThanZero)]
    public int Quantity { get; set; }

    [Required(ErrorMessage = ValidationMessages.MovementTypeRequired)]
    public StockMovementType MovementType { get; set; }

    [Required(ErrorMessage = ValidationMessages.ReasonRequired)]
    [MaxLength(250, ErrorMessage = ValidationMessages.DescriptionMax250)]
    public string Reason { get; set; } = string.Empty;
}
