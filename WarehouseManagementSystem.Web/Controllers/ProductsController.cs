using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Application.Features.Products.Commands;
using WarehouseManagementSystem.Application.Features.Products.DTOs;
using WarehouseManagementSystem.Application.Features.Products.Queries;

namespace WarehouseManagementSystem.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new CreateProductCommand { Dto = dto });
                TempData["Message"] = "Product created successfully!";
                TempData["IsSuccess"] = "True";
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return NotFound();

            var dto = await _mediator.Send(new GetProductByIdQuery { Id = id });
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateProductCommand { Dto = dto });
                TempData["Message"] = "Product updated successfully!";
                TempData["IsSuccess"] = "True";
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "Please select a file to upload.";
                TempData["IsSuccess"] = "False";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var stream = file.OpenReadStream();
                var command = new ImportProductsFromCsvCommand { CsvStream = stream };

                // The command now returns our detailed result object
                var result = await _mediator.Send(command);

                // Build the feedback message based on the result
                var messageBuilder = new System.Text.StringBuilder();

                if (result.SuccessCount > 0)
                {
                    messageBuilder.Append($"{result.SuccessCount} record(s) processed successfully.");
                }

                if (result.HasErrors)
                {
                    TempData["IsSuccess"] = "False"; // Mark as failure if there are any errors
                    messageBuilder.Append($"<br/>However, {result.Errors.Count} error(s) were found:");
                    messageBuilder.Append("<ul class='mt-2'>");
                    foreach (var error in result.Errors)
                    {
                        messageBuilder.Append($"<li>{error}</li>");
                    }
                    messageBuilder.Append("</ul>");
                }
                else
                {
                    TempData["IsSuccess"] = "True";
                    messageBuilder.Append(" The import was completed without any errors.");
                }

                TempData["Message"] = messageBuilder.ToString();
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"An unexpected system error occurred during import: {ex.Message}";
                TempData["IsSuccess"] = "False";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());
            return Json(new { data = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });
            return Json(new { success = true, message = "Product deleted successfully." });
        }
    }
}