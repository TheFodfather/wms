using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Application.Products.DTOs
{
    public class UpdateProductDto
    {
        public int Id { get; set; }

        [Required]
        public required string Sku { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [DisplayName("Manufacturer")]
        public required string ManufacturerName { get; set; }

        [Required]
        [DisplayName("Category")]
        public required string CategoryName { get; set; }

        [Range(0, double.MaxValue)]
        [DisplayName("Cost Price")]
        public decimal CostPrice { get; set; }

        [Range(0, double.MaxValue)]
        [DisplayName("Sell Price")]
        public decimal SellPrice { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Quantity")]
        public int Qty { get; set; }
    }
}
