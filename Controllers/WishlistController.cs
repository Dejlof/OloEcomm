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


        public WishlistController(IWishlistRepository wishlistRepository, IProductReposity productReposity, UserManager<User> userManager) 
        {
        _wishlistRepository = wishlistRepository;
            _productReposity = productReposity;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var wishlist = await _wishlistRepository.GetAllAsync(user);
         var wishlistDto = wishlist.Select(s => s.ToWishlistDto()).ToList(); 
        return Ok(wishlistDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         var wishlist = await _wishlistRepository.GetByIdAsync(id);
            if (wishlist == null) 
            {
                return NotFound("Wishlist not found");
            }
          return Ok(wishlist.ToWishlistDto());
         }
        
        [HttpPost("{productId:int}")]
        [Authorize]
        public async Task<IActionResult> AddWishlist([FromRoute]int productId, [FromBody] CreateWishlistDto wishlistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var product = await _productReposity.productExist(productId);
            if (product == null)
            {
                return BadRequest("Product not found");
            }

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                return Unauthorized("User not permitted");
            }

            var wishlistModel = wishlistDto.CreateToWishlistDto(productId);
            wishlistModel.UserId = appUser.Id;
            wishlistModel.WishlistItem = product.Name;
            wishlistModel.UserWishlist = user;
            await _wishlistRepository.CreateAsync(wishlistModel);

            return CreatedAtAction(nameof(GetById), new { id = productId }, wishlistModel.ToWishlistDto());

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteById(int id) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }

            var wishList = await _wishlistRepository.DeleteAsync(id, user);

            if (wishList == null) 
            { 
             return NotFound();
            }

            return Ok($"Wishlist with {id} deleted");
        }
    }
}
