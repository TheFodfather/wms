using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Domain.Entities;

namespace WarehouseManagementSystem.Application.Interfaces
{
    public interface IWarehouseDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<Category> Categories { get; }
        DbSet<Manufacturer> Manufacturers { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
