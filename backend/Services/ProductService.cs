using System.Collections.Generic;
using System.Threading.Tasks;
using Shopping_Pet.DTOs.Products;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<(bool Success, string Message, int ProductId)> CreateProductAsync(CreateProductModel model)
        {
            var existing = await _productRepository.GetByNameAsync(model.Name);
            if (existing != null)
            {
                return (false, "Tên sản phẩm đã tồn tại", 0);
            }

            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if (category == null)
            {
                return (false, "Danh mục không tồn tại", 0);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            return (true, "Tạo sản phẩm thành công", product.Id);
        }

        public async Task<List<GetProductModel>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllWithCategoryAsync();

            return products.Select(p => new GetProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name, 
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                AverageRating = p.Reviews?.Any() == true ? Math.Round(p.Reviews.Average(r => r.Rating), 1) : 0
            }).ToList();
        }

        public async Task<GetProductModel?> GetProductByIdAsync(int id)
        {
            var p = await _productRepository.GetByIdAsync(id);
            if (p == null) return null;

            return new GetProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                AverageRating = p.Reviews?.Any() == true ? Math.Round(p.Reviews.Average(r => r.Rating), 1) : 5, 
                ReviewCount = p.Reviews?.Count() ?? 0
            };

        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(int id, UpdateProductModel model)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Sản phẩm không tồn tại");

            var exists = await _productRepository.GetByNameAsync(model.Name);
            if (exists != null && exists.Id != id)
                return (false, "Tên sản phẩm đã tồn tại");

            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if (category == null)
                return (false, "Danh mục không tồn tại");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return (true, "Cập nhật sản phẩm thành công");
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Sản phẩm không tồn tại");

            await _productRepository.DeleteAsync(id);
            return (true, "Xóa sản phẩm thành công");
        }
    }
}
