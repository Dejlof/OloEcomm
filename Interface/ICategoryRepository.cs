using OloEcomm.Dtos.Category;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Category> CreateCategoryAsync (Category categoryModel); 

        Task<Category?> UpdateCategoryAsync (int id, UpdateCategoryDto categoryDto);

        Task <Category?> DeleteCategoryAsync (int id);
    }
}
