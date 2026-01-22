using System.ComponentModel.DataAnnotations;

namespace StockCore.Entities
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
