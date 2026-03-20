using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Categories;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });
            }

            var (success, message, categoryId) = await _categoryService.CreateCategoryAsync(model);
            if (success)
            {
                return Ok(new { Message = message, CategoryId = categoryId });
            }

            return BadRequest(new { Message = message });
        }
        
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoriesPublic()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { Message = "Không tìm thấy danh mục" });

            return Ok(category);
        }



        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });
            }

            var (success, message) = await _categoryService.UpdateCategoryAsync(id, model);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var (success, message) = await _categoryService.DeleteCategoryAsync(id);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
    }
}
