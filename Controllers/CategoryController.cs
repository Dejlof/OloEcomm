using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data;
using OloEcomm.Dtos.Category;
using OloEcomm.Interface;
using OloEcomm.Mappers;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository; 
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() 
        {
            _logger.LogInformation("Fetching all categories");
        var category = await _categoryRepository.GetAllCategoriesAsync();
        var categoryDto = category.Select(s=>s.ToCategoryDto()).ToList();
         return Ok(categoryDto);

        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> GetById(int id) 
        {
            if (!ModelState.IsValid) 
            { 
                _logger.LogWarning("Invalid model state for category request.");
            return BadRequest(ModelState);
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null) 
            {
                _logger.LogWarning("Category with id: {Id} not found.", id);    
                return NotFound();
            }
            _logger.LogInformation("Fetching category with id: {Id}", id);
            return Ok(category.ToCategoryDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for category request.");
                return BadRequest(ModelState);
            }

            var categoryModel = categoryDto.ToCreateCategoryDto();
            _logger.LogInformation("Creating category: {Category}", categoryModel.Name);
            await _categoryRepository.CreateCategoryAsync(categoryModel);
            return CreatedAtAction(nameof(GetById), new { id = categoryModel.Id }, categoryModel.ToCategoryDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryDto categoryDto) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for category request.");
                return BadRequest(ModelState);
            }
            var category = await _categoryRepository.UpdateCategoryAsync(id, categoryDto);

            if (category == null)
            {
                _logger.LogWarning("Category with id: {Id} not found.", id);
                return NotFound();
            }
            _logger.LogInformation("Updating category with id: {Id}", id);
            return Ok(category.ToCategoryDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for category request.");
                return BadRequest(ModelState);
            }
            var category = await _categoryRepository.DeleteCategoryAsync(id);
            if (category == null)
            { 
                _logger.LogWarning("Category with id: {Id} not found.", id);
                return NotFound(); 
            }

            _logger.LogInformation("Deleting category with id: {Id}", id);  
            return Ok($"Category with id: {id} sucessfully deleted");
        }
    }
}
