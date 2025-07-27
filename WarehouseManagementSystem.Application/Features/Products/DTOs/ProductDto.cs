namespace WarehouseManagementSystem.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int Qty { get; set; }
        public required string Manufacturer { get; set; }
        public required string Category { get; set; }
        public decimal PriceMarginPercentage { get; set; }
    }
}
