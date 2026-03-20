using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllWithCategoryAsync();

        Task<Product?> GetByNameAsync(string name);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
