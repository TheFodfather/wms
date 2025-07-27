using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Application.Interfaces;
using WarehouseManagementSystem.Application.Products.DTOs;

namespace WarehouseManagementSystem.Application.Products.Queries
{
    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>> { }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IWarehouseDbContext _context;

        public GetAllProductsQueryHandler(IWarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Manufacturer = p.Manufacturer.Name,
                    Category = p.Category.Name,
                    CostPrice = p.CostPrice,
                    SellPrice = p.SellPrice,
                    Qty = p.Qty,
                    PriceMarginPercentage = p.SellPrice > 0 ? ((p.SellPrice - p.CostPrice) / p.SellPrice) * 100 : 0
                })
                .ToListAsync(cancellationToken);
        }
    }
}
