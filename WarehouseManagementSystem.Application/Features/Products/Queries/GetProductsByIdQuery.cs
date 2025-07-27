using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Application.Features.Products.DTOs;
using WarehouseManagementSystem.Application.Interfaces;

namespace WarehouseManagementSystem.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<UpdateProductDto>
    {
        public int Id { get; set; }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, UpdateProductDto?>
    {
        private readonly IWarehouseDbContext _context;

        public GetProductByIdQueryHandler(IWarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Where(p => p.Id == request.Id)
                .Select(p => new UpdateProductDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    ManufacturerName = p.Manufacturer.Name,
                    CategoryName = p.Category.Name,
                    CostPrice = p.CostPrice,
                    SellPrice = p.SellPrice,
                    Qty = p.Qty
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
