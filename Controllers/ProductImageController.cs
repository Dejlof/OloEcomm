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
        public ProductImageController(IFileService fileService, IProductImageRepository productImageRepository, IProductReposity productReposity)
        {
            _fileService = fileService;
            _productImageRepository = productImageRepository;
            _productReposity = productReposity;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productImages = await _productImageRepository.GetAllAsync();
            var productImagesDto = productImages.Select(s => s.ToProductImageDto()).ToList();
            return Ok(productImagesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productImage = await _productImageRepository.GetByIdAsync(id);

            if (productImage == null)
            {
                return NotFound();
            }

            return Ok(productImage.ToProductImageDto());
        }

        [HttpPost("{productId:int}/upload")]
        public async Task<IActionResult> UploadImage([FromRoute] int productId, [FromForm] CreateProductImageDto productImageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                return BadRequest("Product does not exist");
            }
            string imageUrl;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (productImageDto.imageFile == null)
            {
                return BadRequest("Image file is required.");
            }

            try
            {
                imageUrl = await _fileService.SaveFileAsync(productImageDto.imageFile, allowedExtensions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Image upload failed: {ex.Message}");
            }

            var productImage = new ProductImage
            {
                Url = imageUrl,
                ProductId = productId
            };

            var createdImage = await _productImageRepository.CreateImageAsync(productImage);

            return CreatedAtAction(nameof(GetById), new { Id = productId }, createdImage.ToProductImageDto());
        }

        [HttpPut("{id:int}/update ")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, [FromForm] UpdateProductImage productImageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imageUrl;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (productImageDto.imageFile == null)
            {
                return BadRequest("Image file is required.");
            }
            try
            {
                imageUrl = await _fileService.SaveFileAsync(productImageDto.imageFile, allowedExtensions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Image upload failed: {ex.Message}");
            }

            var productImage = new ProductImage
            {
                Url = imageUrl
            };
            var updatedImage = await _productImageRepository.UpdateImageAsync(id, productImage);

            return Ok(updatedImage.ToProductImageDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productImage = await _productImageRepository.GetByIdAsync(id);
            if (productImage == null)
            {
                return NotFound("Image not found.");
            }
            try
            {
                var isDeleted = await _fileService.DeleteFileAsync(productImage.Url);
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete the image file from the server.");
                }

                await _productImageRepository.DeleteImageAsync(id);
                return Ok("Image deleted sucessfully");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
