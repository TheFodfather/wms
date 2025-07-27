using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Application.Interfaces;
using WarehouseManagementSystem.Application.Products.DTOs;
using WarehouseManagementSystem.Domain.Entities;

namespace WarehouseManagementSystem.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<int>
    {
        public required CreateProductDto Dto { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IWarehouseDbContext _context;

        public CreateProductCommandHandler(IWarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Find or Create Category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == request.Dto.CategoryName, cancellationToken);
            if (category == null)
            {
                category = new Category { Name = request.Dto.CategoryName };
                _context.Categories.Add(category);
            }

            // Find or Create Manufacturer
            var manufacturer = await _context.Manufacturers
                .FirstOrDefaultAsync(m => m.Name == request.Dto.ManufacturerName, cancellationToken);
            if (manufacturer == null)
            {
                manufacturer = new Manufacturer { Name = request.Dto.ManufacturerName };
                _context.Manufacturers.Add(manufacturer);
            }

            var product = new Product
            {
                Sku = request.Dto.Sku,
                Name = request.Dto.Name,
                CostPrice = request.Dto.CostPrice,
                SellPrice = request.Dto.SellPrice,
                Qty = request.Dto.Qty,
                Category = category,
                Manufacturer = manufacturer,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
