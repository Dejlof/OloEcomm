using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Dtos.Cart;
using OloEcomm.Extensions;
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
        private readonly UserManager<User> _userManager;
        public ShoppingCartController( IShoppingCartRepository shopRepo, IProductReposity productReposity, UserManager<User> userManager)
        { 
            _shopRepo = shopRepo;
            _productReposity = productReposity;
            _userManager = userManager;
        }

        [HttpGet("GetMyCarts")]
        [Authorize]
        public async Task<IActionResult> GetMyCarts()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var carts = await _shopRepo.GetCartsAsync(user);
            var cartsDto = carts.Select(s => s.ToCartItemDto()).ToList();
            return Ok(cartsDto);

        }

        [HttpGet("GetUserCarts")]
        public async Task<IActionResult> GetUserCarts(string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var carts = await _shopRepo.GetCartsAsync(username);
            var cartsDto = carts.Select(s => s.ToCartItemDto()).ToList();
            return Ok(cartsDto);

        }

        [HttpGet("MyCartItem/{productId:int}")]
        [Authorize]
        public async Task<IActionResult> GetMyCartProductById(int productId)
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
            var cartItem = await _shopRepo.GetProductCartIdAsync(productId,  user);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem.ToCartItemDto());
        }

        [HttpGet("UserCartItem/{productId:int}")]
        public async Task<IActionResult> GetUserCartProductById([FromRoute]int productId, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            var cartItem = await _shopRepo.GetProductCartIdAsync(productId, username);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem.ToCartItemDto());
        }




        [HttpPost("CreateMyCartItem/{productId:int}")]
        [Authorize]
        public async Task<IActionResult> AddToMyCart([FromRoute] int productId, [FromBody] CreateCartItemDto cartItemDto)
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

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                return Unauthorized("User not permitted");
            }

            var cartItem = cartItemDto.ToCreateCartItemDto(productId);
            cartItem.UserId = appUser.Id;
           

             await _shopRepo.AddToCartAsync(cartItem.ProductId, cartItem.Quantity, appUser.Id, user, cartItem.ProductAdded);

            return Ok($"ProductId {cartItem.ProductAdded} added to cart successfully with {cartItem.Quantity} quantity");
        }

        [HttpPost("CreateUserCartItem/{productId:int}")]
        public async Task<IActionResult> AddToUserCart([FromRoute] int productId, [FromBody] CreateCartItemDto cartItemDto, string username)
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

            
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return Unauthorized("User not permitted");
            }

            var cartItem = cartItemDto.ToCreateCartItemDto(productId);
            cartItem.UserId = appUser.Id;
            cartItem.ProductAdded = cartItem.Product?.Name;




            await _shopRepo.AddToCartAsync(cartItem.ProductId, cartItem.Quantity, appUser.Id, username, cartItem.ProductAdded);

            return Ok($"ProductId {cartItem.ProductAdded} added to cart sucessfully with {cartItem.Quantity} quantity");
        }


        [HttpPut("UpdateMyCartItem/{productId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProductCart([FromRoute] int productId, [FromBody] UpdateCartItemDto cartItemDto)
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
            var cartItem = await _shopRepo.UpdateCartAsync(productId, cartItemDto.ToUpdateCartItemDto(), user);
          
  

            if (cartItem == null) 
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok(cartItem.ToCartItemDto());

        }

        [HttpPut("UpdateUserCartItem/{productId:int}")]
        public async Task<IActionResult> UpdateUserProductCart([FromRoute] int productId, [FromBody] UpdateCartItemDto cartItemDto, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _shopRepo.UpdateCartAsync(productId, cartItemDto.ToUpdateCartItemDto(), username);
            
            if (cartItem == null)
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok(cartItem.ToCartItemDto());

        }

        [HttpDelete("RemoveMyProduct/{productId:int}")]
        [Authorize]
        public async Task<IActionResult> RemoveMyProductCart([FromRoute,] int productId)
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
            var cart = await _shopRepo.RemovefromCartAsync(productId, user);

            if (cart == null)
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok($"Product with {productId} sucessfully removed");
        }



        [HttpDelete("RemoveUserProduct/{productId:int}")]
        public async Task<IActionResult> RemoveUserProductCart([FromRoute,] int productId, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cart = await _shopRepo.RemovefromCartAsync(productId, username);

            if (cart == null)
            {
                return NotFound($"Product with {productId} not found");
            }

            return Ok($"Product with {productId} sucessfully removed");
        }

        [HttpDelete("/ClearMyCart")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            await _shopRepo.ClearCartAsync(user);

            return Ok("Shopping Carts Cleared");
        }

        [HttpDelete("/ClearUserCart")]
        public async Task<IActionResult> ClearCart(string username)
        {
            await _shopRepo.ClearCartAsync(username);

            return Ok("Shopping Carts Cleared");
        }
        
    }
}
