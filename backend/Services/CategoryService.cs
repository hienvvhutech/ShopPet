using System.Collections.Generic;
using System.Threading.Tasks;
using Shopping_Pet.DTOs.Categories;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<(bool Success, string Message, int CategoryId)> CreateCategoryAsync(CreateCategoryModel model)
        {
            var all = await _categoryRepo.GetAllAsync();
            if (all.Any(c => c.Name == model.Name))
            {
                return (false, "Tên danh mục đã tồn tại", 0);
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _categoryRepo.AddAsync(category);
            return (true, "Tạo danh mục thành công", category.Id);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categoryRepo.GetAllAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(int id, UpdateCategoryModel model)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                return (false, "Danh mục không tồn tại");

            var all = await _categoryRepo.GetAllAsync();
            if (all.Any(c => c.Name == model.Name && c.Id != id))
                return (false, "Tên danh mục đã tồn tại");

            category.Name = model.Name;
            category.Description = model.Description;
            await _categoryRepo.UpdateAsync(category);

            return (true, "Cập nhật danh mục thành công");
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                return (false, "Danh mục không tồn tại");

            if (category.Products != null && category.Products.Any())
                return (false, "Không thể xóa danh mục đang có sản phẩm");

            await _categoryRepo.DeleteAsync(id);
            return (true, "Xóa danh mục thành công");
        }
    }
}
