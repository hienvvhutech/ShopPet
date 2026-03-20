using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Products;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductModel model, IFormFile image)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });

            string imageUrl = null;

            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/{fileName}";
                model.ImageUrl = imageUrl;
            }

            var (success, message, productId) = await _productService.CreateProductAsync(model);
            if (success)
                return Ok(new { Message = message, ProductId = productId });

            return BadRequest(new { Message = message });
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found" });
            }

            return Ok(product); 
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductModel model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });

            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                model.ImageUrl = $"/uploads/{fileName}";
            }

            var (success, message) = await _productService.UpdateProductAsync(id, model);
            if (success) return Ok(new { Message = message });
            return BadRequest(new { Message = message });
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var (success, message) = await _productService.DeleteProductAsync(id);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
    }
}
