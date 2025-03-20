using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Dtos.Product;
using OloEcomm.Dtos.ProductImage;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/ProductImage")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductReposity _productReposity;
        private readonly ILogger<ProductImageController> _logger;
        public ProductImageController(IFileService fileService, IProductImageRepository productImageRepository, IProductReposity productReposity, ILogger<ProductImageController> logger)
        {
            _fileService = fileService;
            _productImageRepository = productImageRepository;
            _productReposity = productReposity;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all product images");
            var productImages = await _productImageRepository.GetAllAsync();
            var productImagesDto = productImages.Select(s => s.ToProductImageDto()).ToList();
            return Ok(productImagesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product image request.");
                return BadRequest(ModelState);
            }
            var productImage = await _productImageRepository.GetByIdAsync(id);

            if (productImage == null)
            {
                _logger.LogWarning("Product image with id: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetching product image with id: {Id}", id);
            return Ok(productImage.ToProductImageDto());
        }

        [HttpPost("{productId:int}/upload")]
        [Authorize(Roles="Admin, Vendor")]
        public async Task<IActionResult> UploadImage([FromRoute] int productId, [FromForm] CreateProductImageDto productImageDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product image request.");   
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                _logger.LogWarning("Product does not exist.");  
                return BadRequest("Product does not exist");
            }
            string imageUrl;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (productImageDto.imageFile == null)
            {
                _logger.LogWarning("Image file is required.");
                return BadRequest("Image file is required.");
            }

            try
            {
                _logger.LogInformation("Uploading image for product: {Product}", productId);
                imageUrl = await _fileService.SaveFileAsync(productImageDto.imageFile, allowedExtensions);
            }
            catch (Exception ex)
            {
                _logger.LogError("Image upload failed: {Message}", ex.Message);
                return BadRequest($"Image upload failed: {ex.Message}");
            }

            var productImage = new ProductImage
            {
                Url = imageUrl,
                ProductId = productId
            };
          _logger.LogInformation("Creating image for product: {Product}", productId);
            var createdImage = await _productImageRepository.CreateImageAsync(productImage);

            return CreatedAtAction(nameof(GetById), new { Id = productId }, createdImage.ToProductImageDto());
        }

        [HttpPut("{id:int}/update ")]
         [Authorize(Roles="Admin, Vendor")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, [FromForm] UpdateProductImage productImageDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product image request.");
                return BadRequest(ModelState);
            }

            string imageUrl;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (productImageDto.imageFile == null)
            {
                _logger.LogWarning("Image file is required.");
                return BadRequest("Image file is required.");
            }
            try
            {
                _logger.LogInformation("Uploading image for product: {Product}", id);
                imageUrl = await _fileService.SaveFileAsync(productImageDto.imageFile, allowedExtensions);
            }
            catch (Exception ex)
            {
                _logger.LogError("Image upload failed: {Message}", ex.Message);
                return BadRequest($"Image upload failed: {ex.Message}");
            }

            var productImage = new ProductImage
            {
                Url = imageUrl
            };
            _logger.LogInformation("Updating image with id: {Id}", id);
            var updatedImage = await _productImageRepository.UpdateImageAsync(id, productImage);

            return Ok(updatedImage.ToProductImageDto());
        }

        [HttpDelete("{id:int}")]
         [Authorize(Roles="Admin, Vendor")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product image request.");
                return BadRequest(ModelState);
            }
            var productImage = await _productImageRepository.GetByIdAsync(id);
            if (productImage == null)
            {
                _logger.LogWarning("Image not found.");
                return NotFound("Image not found.");
            }
            try
            {
            _logger.LogInformation("Deleting image with id: {Id}", id);
                await _productImageRepository.DeleteImageAsync(id);
                return Ok("Image deleted sucessfully");
            }
            catch (Exception ex) 
            {
                _logger.LogError("Error deleting image: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
