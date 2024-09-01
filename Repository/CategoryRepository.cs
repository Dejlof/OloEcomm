using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Dtos.Category;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<Category> CreateCategoryAsync(Category categoryModel)
        {
            await _context.Categories.AddAsync(categoryModel);
            await _context.SaveChangesAsync();
            return categoryModel;   
        }

        public async Task<Category?> DeleteCategoryAsync(int id)
        {
            var categoryModel = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryModel == null) 
            {
                return null;
            }  

            _context.Categories.Remove(categoryModel);
            await _context.SaveChangesAsync();
            return categoryModel;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categoryModels = await _context.Categories.Include(s=>s.Products).ToListAsync();
            return categoryModels;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
          var category = await _context.Categories.Include(s=>s.Products).FirstOrDefaultAsync(x=>x.Id==id);
            if (category == null) 
            {
                return null;
            }
            
            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;

            await _context.SaveChangesAsync();
            return existingCategory;
        }
    }
}
