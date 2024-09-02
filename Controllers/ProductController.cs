using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data;
using OloEcomm.Dtos.Product;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductReposity _productReposity;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductReposity productReposity, ICategoryRepository categoryRepository)
        {
            _productReposity = productReposity;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productReposity.GetAllProductsAsync();
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            return Ok(productsDto);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productReposity.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost("{categoryId:int}")]
        public async Task<IActionResult> CreateProduct([FromRoute] int categoryId, [FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.categoryExists(categoryId))
            {
                return BadRequest("Product does not exist");
            }
            var productModel = productDto.CreateProductDto(categoryId);

            await _productReposity.CreateProductAsync(productModel);

            return CreatedAtAction(nameof(GetById),new {id = categoryId}, productModel.ToProductDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productModel = await _productReposity.UpdateProductAsync(id, productDto.ToUpdateProductDto());

            if (productModel == null) 
            {
                return NotFound();
            }
            return Ok(productModel.ToProductDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productReposity.DeleteProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok($"Product with id: {id} sucessfully deleted");
        }
    }
}
