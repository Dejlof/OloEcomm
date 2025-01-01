using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data;
using OloEcomm.Dtos.Product;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;
using System.Security.Claims;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductReposity _productReposity;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<User> _userManager;
        public ProductController(IProductReposity productReposity, ICategoryRepository categoryRepository, UserManager<User> userManager)
        {
            _productReposity = productReposity;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productReposity.GetAllProductsAsync();
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            return Ok(productsDto);

        }

        [Authorize]
        [HttpGet("GetMyProducts")]
        public async Task<IActionResult> GetMyProducts()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var products = await _productReposity.GetUserProductsAsync(user);
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            return Ok(productsDto);

        }


        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromRoute] int categoryId, [FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.categoryExists(categoryId))
            {
                return BadRequest("Category does not exist");
            }

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);

            var productModel = productDto.CreateProductDto(categoryId);
            productModel.UserId = appUser.Id;

            await _productReposity.CreateProductAsync(productModel);

            return CreatedAtAction(nameof(GetById), new { id = categoryId }, productModel.ToProductDto());
        }

        [Authorize]
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

            var currentUserId = User.GetUsername();
            if (productModel.CreatedBy != currentUserId)
            {
                return Unauthorized("You are not authorized to update this product.");
            }
            return Ok(productModel.ToProductDto());
        }

        [Authorize]
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
        [Authorize]
        [HttpDelete("DeleteMyProduct{id:int}")]
        public async Task<IActionResult> DeleteMyProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var product = await _productReposity.DeleteUserProductAsync(user, id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok($"Product with id: {id} sucessfully deleted");
        }
    }
}
