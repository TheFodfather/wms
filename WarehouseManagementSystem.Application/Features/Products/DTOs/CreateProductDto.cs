using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WarehouseManagementSystem.Application.Features.Products.DTOs
{
    public class CreateProductDto
    {
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
