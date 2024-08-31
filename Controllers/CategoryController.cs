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
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
        var category = await _categoryRepository.GetAllCategoriesAsync();
        var categoryDto = category.Select(s=>s.ToCategoryDto()).ToList();
         return Ok(categoryDto);

        }


        [HttpGet("{id}")]
        public async Task <IActionResult> GetById(int id) 
        {
            if (!ModelState.IsValid) 
            { 
            return BadRequest(ModelState);
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null) 
            {
                return NotFound();
            }
            return Ok(category.ToCategoryDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryModel = categoryDto.ToCreateCategoryDto();
            await _categoryRepository.CreateCategoryAsync(categoryModel);
            return CreatedAtAction(nameof(GetById), new { id = categoryModel.Id }, categoryModel.ToCategoryDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryDto categoryDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = await _categoryRepository.UpdateCategoryAsync(id, categoryDto);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category.ToCategoryDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = await _categoryRepository.DeleteCategoryAsync(id);
            if (category == null)
            { return NotFound(); 
            }

            return Ok($"Category with id: {id} sucessfully deleted");
        }
    }
}
