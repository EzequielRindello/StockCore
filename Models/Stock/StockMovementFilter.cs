using StockCore.Dtos.Enums;

public class StockMovementFilter
{
    public int? ProductId { get; set; }
    public StockMovementType? MovementType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}