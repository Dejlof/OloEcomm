using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Review;
using OloEcomm.Dtos.Wishlist;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Repository;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductReposity _productReposity;



        public WishlistController(IWishlistRepository wishlistRepository, IProductReposity productReposity) 
        {
        _wishlistRepository = wishlistRepository;
            _productReposity = productReposity;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
         var wishlist = await _wishlistRepository.GetAllAsync();
         var wishlistDto = wishlist.Select(s => s.ToWishlistDto()).ToList(); 
        return Ok(wishlistDto);
        }

        [HttpGet("{id:int}")]
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
        public async Task<IActionResult> AddWishlist([FromRoute]int productId, [FromBody] CreateWishlistDto wishlistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            if (!await _productReposity.productExists(productId))
            {
                return BadRequest("Product does not exist");
               
            }
              
            var wishlistModel = wishlistDto.CreateToWishlistDto(productId);   
            await _wishlistRepository.CreateAsync(wishlistModel);

            return CreatedAtAction(nameof(GetById), new { id = productId }, wishlistModel.ToWishlistDto());

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteById(int id) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wishList = await _wishlistRepository.DeleteAsync(id);

            if (wishList == null) 
            { 
             return NotFound();
            }

            return Ok($"Wishlist with {id} deleted");
        }
    }
}
