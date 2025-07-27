using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Sku { get; set; } = default!;
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = default!;
        [MaxLength(100)]
        public string? ManufacturersCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsActive { get; set; }
        public string? Summary { get; set; }
        public decimal Weight { get; set; }
        [MaxLength(10)]
        public string? WeightUnit { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = default!;

        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; } = default!;


        [Column(TypeName = "decimal(18,4)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal SellPrice { get; set; }

        public int Qty { get; set; }
    }
}

