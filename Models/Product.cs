using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockCore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Sku { get; set; } = string.Empty;

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Category? Category { get; set; }
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}
