using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Domain.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
