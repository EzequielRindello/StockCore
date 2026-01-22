using System.ComponentModel.DataAnnotations;

namespace StockCore.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
