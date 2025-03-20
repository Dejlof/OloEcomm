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
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductReposity productReposity, ICategoryRepository categoryRepository, UserManager<User> userManager, ILogger<ProductController> logger)
        {
            _productReposity = productReposity;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _logger = logger;
        }

    
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("Fetching all products");
            var products = await _productReposity.GetAllProductsAsync();
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            return Ok(productsDto);

        }

        
        [HttpGet("GetMyProducts")]
        [Authorize(Roles ="Admin,Vendor")]
        public async Task<IActionResult> GetMyProducts()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to product.");
                return Unauthorized();
            }
            var products = await _productReposity.GetUserProductsAsync(user);
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            _logger.LogInformation("Fetching products for user: {User}", user);
            return Ok(productsDto);

        }

        
        [HttpGet("GetVendorProducts")]
        
        public async Task<IActionResult> GetUsersProduct(string username) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }

            var products = await _productReposity.GetUserProductsAsync(username);
            var productsDto = products.Select(s => s.ToProductDto()).ToList();
            _logger.LogInformation("Fetching products for user: {User}", username);
            return Ok(productsDto);
        }


        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger .LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }
            var product = await _productReposity.GetById(id);

            if (product == null)
            {
                _logger.LogWarning("Product with id: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetching product with id: {Id}", id);
            return Ok(product.ToProductDto());
        }

        [HttpPost("{categoryId:int}")]
        [Authorize(Roles ="Admin,Vendor")]
        public async Task<IActionResult> CreateProduct([FromRoute] int categoryId, [FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.categoryExists(categoryId))
            {
                _logger.LogWarning("Category does not exist.");
                return BadRequest("Category does not exist");
            }

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);

            var productModel = productDto.CreateProductDto(categoryId);
            productModel.UserId = appUser.Id;

            _logger.LogInformation("Creating product: {Product}", productModel.Name);
            await _productReposity.CreateProductAsync(productModel);

            return CreatedAtAction(nameof(GetById), new { id = categoryId }, productModel.ToProductDto());
        }

        
        [HttpPut("{id:int}")]
        [Authorize(Roles ="Admin,Vendor")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }
            var productModel = await _productReposity.UpdateProductAsync(id, productDto.ToUpdateProductDto());

            if (productModel == null) 
            {
                _logger.LogWarning("Product with id: {Id} not found.", id);
                return NotFound();
            }

            var currentUserId = User.GetUsername();
            if (productModel.CreatedBy != currentUserId)
            {
                _logger.LogWarning("Unauthorized access attempt to product.");
                return Unauthorized("You are not authorized to update this product.");
            }
            _logger.LogInformation("Updating product with id: {Id}", id);
            return Ok(productModel.ToProductDto());
        }

       
        [HttpDelete("{id:int}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }
            var product = await _productReposity.DeleteProductAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id: {Id} not found.", id);
                return NotFound();
            }

             _logger.LogInformation("Deleting product with id: {Id}", id);
            return Ok($"Product with id: {id} sucessfully deleted");
        }
        
        [HttpDelete("DeleteMyProduct{id:int}")]
        [Authorize(Roles ="Admin,Vendor")]
        public async Task<IActionResult> DeleteMyProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product request.");
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to product.");
                return Unauthorized();
            }
            var product = await _productReposity.DeleteUserProductAsync(user, id);
            if (product == null)
            {
                _logger.LogWarning("Product with id: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Deleting product with id: {Id}", id);

            return Ok($"Product with id: {id} sucessfully deleted");
        }
    }
}
