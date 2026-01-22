using StockCore.Dtos.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockCore.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Required]
        public StockMovementType MovementType { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product? Product { get; set; }
    }
}
