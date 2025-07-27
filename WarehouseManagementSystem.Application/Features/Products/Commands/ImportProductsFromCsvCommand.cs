using CsvHelper.Configuration;
using CsvHelper;
using MediatR;
using System.Globalization;
using WarehouseManagementSystem.Application.Interfaces;
using WarehouseManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Application.Features.Products.DTOs;

namespace WarehouseManagementSystem.Application.Features.Products.Commands
{

    public class ImportProductsFromCsvCommand : IRequest<ImportResultDto>
    {
        public Stream? CsvStream { get; set; }
    }

    public class ImportProductsFromCsvCommandHandler : IRequestHandler<ImportProductsFromCsvCommand, ImportResultDto>
    {
        private readonly IWarehouseDbContext _context;

        public ImportProductsFromCsvCommandHandler(IWarehouseDbContext context)
        {
            _context = context;
        }


        public async Task<ImportResultDto> Handle(ImportProductsFromCsvCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportResultDto();
            var productsToProcess = new List<Product>();
            // Ensure the stream is not null before proceeding
            if (request.CsvStream == null)
            {
                throw new ArgumentNullException(nameof(request.CsvStream), "The CSV stream cannot be null.");
            }

            var existingCategories = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var existingManufacturers = await _context.Manufacturers.ToDictionaryAsync(m => m.Name, m => m, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var existingProducts = await _context.Products.ToDictionaryAsync(p => p.Sku, p => p, StringComparer.OrdinalIgnoreCase, cancellationToken);

            using var reader = new StreamReader(request.CsvStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", "")
            };
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<dynamic>().ToList();

            int rowNumber = 1; // Start at 1 for user-friendly row numbers (accommodating header)

            foreach (var record in records)
            {
                rowNumber++; // Increment for each data row
                var recordDict = (IDictionary<string, object>)record;

                try
                {
                    // --- VALIDATION ---
                    string? sku = recordDict["sku"] as string;
                    if (string.IsNullOrWhiteSpace(sku))
                    {
                        result.Errors.Add($"Row {rowNumber}: SKU is missing or empty. This row was skipped.");
                        continue;
                    }

                    string? name = recordDict["name"] as string;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        result.Errors.Add($"Row {rowNumber} (SKU: {sku}): Product Name is missing. This row was skipped.");
                        continue;
                    }


                    string? categoryName = recordDict["category"] as string;
                    if (string.IsNullOrWhiteSpace(categoryName))
                    {
                        result.Errors.Add($"Row {rowNumber} (SKU: {sku}): Category is missing. This row was skipped.");
                        continue;
                    }

                    string? manufacturerName = recordDict["manufacturer"] as string;
                    if (string.IsNullOrWhiteSpace(manufacturerName))
                    {
                        result.Errors.Add($"Row {rowNumber} (SKU: {sku}): Manufacturer is missing. This row was skipped.");
                        continue;
                    }

                    if (!existingCategories.TryGetValue(categoryName, out var category))
                    {
                        category = new Category { Name = categoryName };
                        _context.Categories.Add(category);
                        existingCategories.Add(categoryName, category);
                    }

                    if (!existingManufacturers.TryGetValue(manufacturerName, out var manufacturer))
                    {
                        manufacturer = new Manufacturer { Name = manufacturerName };
                        _context.Manufacturers.Add(manufacturer);
                        existingManufacturers.Add(manufacturerName, manufacturer);
                    }

                    if (!existingProducts.TryGetValue(sku, out var product))
                    {
                        product = new Product { Sku = sku, DateCreated = DateTime.UtcNow };
                        _context.Products.Add(product);
                        existingProducts.Add(sku, product);
                    }


                    // Map properties
                    product.Name = name;
                    product.ManufacturersCode = recordDict["manufacturerscode"] as string;
                    product.DateUpdated = DateTime.UtcNow;
                    product.IsActive = recordDict["isactive"] as string == "1";
                    product.Summary = recordDict["summary"] as string;
                    product.Weight = decimal.TryParse(recordDict["weight"] as string, out var w) ? w : 0;
                    product.WeightUnit = recordDict["weightunit"] as string;
                    product.CostPrice = decimal.TryParse((recordDict["costprice"] as string)?.Replace("£", ""), out var cp) ? cp : 0;
                    product.SellPrice = decimal.TryParse((recordDict["sellprice"] as string)?.Replace("£", ""), out var sp) ? sp : 0;
                    product.Qty = int.TryParse(recordDict["qty"] as string, out var q) ? q : 0;
                    product.Category = category;
                    product.Manufacturer = manufacturer;

                    // If all checks pass, mark for success
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    // Catch any other parsing or unexpected errors
                    result.Errors.Add($"Row {rowNumber}: An unexpected error occurred. This row was skipped. Details: {ex.Message}");
                    continue;
                }
            }

            // Only save changes if there were successful records
            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
