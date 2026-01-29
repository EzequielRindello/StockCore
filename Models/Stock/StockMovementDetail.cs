using StockCore.Dtos.Enums;

public class StockMovementDetail
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public StockMovementType MovementType { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}