using Microsoft.AspNetCore.Mvc.Rendering;

public class StockMovementFormView
{
    public StockMovementForm StockMovement { get; set; } = new();
    public List<SelectListItem> Products { get; set; } = new();
}