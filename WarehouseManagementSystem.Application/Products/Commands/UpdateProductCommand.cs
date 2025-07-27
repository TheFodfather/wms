using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Application.Interfaces;
using WarehouseManagementSystem.Application.Products.DTOs;
using WarehouseManagementSystem.Domain.Entities;

namespace WarehouseManagementSystem.Application.Products.Commands
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public required UpdateProductDto Dto { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IWarehouseDbContext _context;

        public UpdateProductCommandHandler(IWarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Dto.Id, cancellationToken);

            if (product == null)
            {
                return Unit.Value;
            }

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

            product.Sku = request.Dto.Sku;
            product.Name = request.Dto.Name;
            product.CostPrice = request.Dto.CostPrice;
            product.SellPrice = request.Dto.SellPrice;
            product.Qty = request.Dto.Qty;
            product.Category = category;
            product.Manufacturer = manufacturer;
            product.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}