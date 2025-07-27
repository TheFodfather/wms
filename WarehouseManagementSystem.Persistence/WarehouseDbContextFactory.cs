using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WarehouseManagementSystem.Persistence
{
    /// <summary>
    /// This factory is used by the Entity Framework Core tools (e.g., for creating migrations)
    /// to create a DbContext instance at design time. It manually sets up the configuration
    // to read the connection string from appsettings.json in the Web project.
    /// </summary>
    public class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
    {
        public WarehouseDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WarehouseManagementSystem.Web"))
                .AddJsonFile("appsettings.json")
                .Build();

            // Get the connection string from the configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);


            return new WarehouseDbContext(optionsBuilder.Options);
        }
    }
}