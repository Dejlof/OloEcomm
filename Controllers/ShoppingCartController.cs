using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Dtos.Cart;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/ShoppingCart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository _shopRepo;
        private readonly IProductReposity _productReposity;
        public ShoppingCartController( IShoppingCartRepository shopRepo, IProductReposity productReposity)
        { 
            _shopRepo = shopRepo;
            _productReposity = productReposity;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts ()
        {
        var carts = await _shopRepo.GetCartsAsync();
        var cartsDto = carts.Select(s=>s.ToCartItemDto()).ToList(); 
            return Ok(cartsDto);
        
        }

        [HttpGet("CartItem/{productId:int}")]
        public async Task<IActionResult> GetCartProductById(int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _shopRepo.GetProductCartIdAsync(productId);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem.ToCartItemDto());
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddToCart([FromRoute] int productId, [FromBody] CreateCartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                return BadRequest("Product does not exist");

            }

            cartItemDto.Quantity = cartItemDto.Quantity > 0 ? cartItemDto.Quantity : 1;

            var cartItem = cartItemDto.ToCreateCartItemDto(productId);
            await _shopRepo.AddToCartAsync( cartItem.ProductId, cartItem.Quantity);

            return Ok($"ProductId {productId} added to cart sucessfully with {cartItem.Quantity} quantity");
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateProductCart([FromRoute] int productId, [FromBody] UpdateCartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _shopRepo.UpdateCartAsync(productId, cartItemDto.ToUpdateCartItemDto());

            if (cartItem == null) 
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok(cartItem.ToCartItemDto());

        }

        [HttpDelete("RemoveProduct/{productId:int}")]
        public async Task<IActionResult> RemoveProductCart([FromRoute,] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cart = await _shopRepo.RemovefromCartAsync(productId);

            if (cart == null)
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok($"Product with {productId} sucessfully removed");
        }

        [HttpDelete("/ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            await _shopRepo.ClearCartAsync();

            return Ok("Shopping Carts Cleared");
        }
        
    }
}
