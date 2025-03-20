using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Review;
using OloEcomm.Dtos.Wishlist;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;
using OloEcomm.Repository;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductReposity _productReposity;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(
            IWishlistRepository wishlistRepository, 
            IProductReposity productReposity, 
            UserManager<User> userManager,
            ILogger<WishlistController> logger) 
        {
            _wishlistRepository = wishlistRepository;
            _productReposity = productReposity;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = User.GetUsername();
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to wishlist.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching wishlist for user: {User}", user);
            var wishlist = await _wishlistRepository.GetAllAsync(user);
            var wishlistDto = wishlist.Select(s => s.ToWishlistDto()).ToList(); 

            return Ok(wishlistDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for wishlist request.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Fetching wishlist item with ID: {WishlistId}", id);
            var wishlist = await _wishlistRepository.GetByIdAsync(id);
            if (wishlist == null) 
            {
                _logger.LogWarning("Wishlist with ID {WishlistId} not found.", id);
                return NotFound("Wishlist not found");
            }

            return Ok(wishlist.ToWishlistDto());
        }
        
        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddWishlist([FromRoute]int productId, [FromBody] CreateWishlistDto wishlistDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while adding to wishlist.");
                return BadRequest(ModelState);
            }

            var product = await _productReposity.productExist(productId);
            if (product == null)
            {
                _logger.LogError("Attempted to add a non-existent product {ProductId} to wishlist.", productId);
                return BadRequest("Product not found");
            }

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                _logger.LogWarning("Unauthorized wishlist addition attempt by user {User}.", user);
                return Unauthorized("User not permitted");
            }

            _logger.LogInformation("Adding product {ProductId} to wishlist for user {User}.", productId, user);
            var wishlistModel = wishlistDto.CreateToWishlistDto(productId);
            wishlistModel.UserId = appUser.Id;
            wishlistModel.WishlistItem = product.Name;
            wishlistModel.UserWishlist = user;
            await _wishlistRepository.CreateAsync(wishlistModel);

            return CreatedAtAction(nameof(GetById), new { id = productId }, wishlistModel.ToWishlistDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while deleting wishlist item.");
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();
            if (user == null)
            {
                _logger.LogWarning("Unauthorized wishlist deletion attempt.");
                return Unauthorized();
            }

            _logger.LogInformation("Attempting to delete wishlist item {WishlistId} for user {User}.", id, user);
            var wishList = await _wishlistRepository.DeleteAsync(id, user);

            if (wishList == null) 
            {
                _logger.LogWarning("Wishlist item {WishlistId} not found for deletion.", id);
                return NotFound();
            }

            _logger.LogInformation("Wishlist item {WishlistId} deleted successfully for user {User}.", id, user);
            return Ok($"Wishlist with ID {id} deleted.");
        }
    }
}
