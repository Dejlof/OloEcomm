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
        private readonly ILogger<ShoppingCartController> _logger;
        public ShoppingCartController( IShoppingCartRepository shopRepo, IProductReposity productReposity, UserManager<User> userManager, ILogger<ShoppingCartController> logger)
        { 
            _shopRepo = shopRepo;
            _productReposity = productReposity;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("GetMyCarts")]
        public async Task<IActionResult> GetMyCarts()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogError("User not found: {User}", user);
                return Unauthorized();
            }
            var carts = await _shopRepo.GetCartsAsync(user);
            var cartsDto = carts.Select(s => s.ToCartItemDto()).ToList();
            _logger.LogInformation("User carts retrieved: {User}", user);
            return Ok(cartsDto);

        }

        [HttpGet("GetUserCarts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserCarts(string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            var carts = await _shopRepo.GetCartsAsync(username);
            var cartsDto = carts.Select(s => s.ToCartItemDto()).ToList();
            _logger.LogInformation("User carts retrieved: {User}", username);
            return Ok(cartsDto);

        }

        [HttpGet("MyCartItem/{productId:int}")]
        
        public async Task<IActionResult> GetMyCartProductById(int productId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogError("User not found: {User}", user);
                return Unauthorized();
            }
            var cartItem = await _shopRepo.GetProductCartIdAsync(productId,  user);
            if (cartItem == null)
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound();
            }

             _logger.LogInformation("Product retrieved: {ProductId}", productId);
            return Ok(cartItem.ToCartItemDto());
        }

        [HttpGet("UserCartItem/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserCartProductById([FromRoute]int productId, string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
          
            var cartItem = await _shopRepo.GetProductCartIdAsync(productId, username);
            if (cartItem == null)
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound();
            }

            _logger.LogInformation("Product retrieved: {ProductId}", productId);

            return Ok(cartItem.ToCartItemDto());
        }


        [HttpPost("CreateMyCartItem/{productId:int}")]
       
        public async Task<IActionResult> AddToMyCart([FromRoute] int productId, [FromBody] CreateCartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return BadRequest("Product does not exist");

            }

            cartItemDto.Quantity = cartItemDto.Quantity > 0 ? cartItemDto.Quantity : 1;

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                _logger.LogError("User not found: {User}", user);
                return Unauthorized("User not permitted");
            }

            var cartItem = cartItemDto.ToCreateCartItemDto(productId);
            cartItem.UserId = appUser.Id;
           
              
             await _shopRepo.AddToCartAsync(cartItem.ProductId, cartItem.Quantity, appUser.Id, user, cartItem.ProductAdded);

             
             _logger.LogInformation("Product added to cart: {ProductId}", cartItem.ProductId);
            return Ok($"ProductId {cartItem.ProductAdded} added to cart successfully with {cartItem.Quantity} quantity");
        }

        [HttpPost("CreateUserCartItem/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddToUserCart([FromRoute] int productId, [FromBody] CreateCartItemDto cartItemDto, string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return BadRequest("Product does not exist");

            }
          



            cartItemDto.Quantity = cartItemDto.Quantity > 0 ? cartItemDto.Quantity : 1;

            
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                _logger.LogError("User not found: {User}", username);
                return Unauthorized("User not permitted");
            }

            _logger.LogInformation("Product added to cart: {ProductId}", productId);
            var cartItem = cartItemDto.ToCreateCartItemDto(productId);
            cartItem.UserId = appUser.Id;
            cartItem.ProductAdded = cartItem.Product?.Name;




            await _shopRepo.AddToCartAsync(cartItem.ProductId, cartItem.Quantity, appUser.Id, username, cartItem.ProductAdded);

          _logger.LogInformation("Product added to cart: {ProductId}", cartItem.ProductId);
            return Ok($"ProductId {cartItem.ProductAdded} added to cart sucessfully with {cartItem.Quantity} quantity");
        }


        [HttpPut("UpdateMyCartItem/{productId:int}")]
        
        public async Task<IActionResult> UpdateMyProductCart([FromRoute] int productId, [FromBody] UpdateCartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogError("User not found: {User}", user);
                return Unauthorized();
            }
            var cartItem = await _shopRepo.UpdateCartAsync(productId, cartItemDto.ToUpdateCartItemDto(), user);
          
  

            if (cartItem == null) 
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound($"Product with {productId} not found");
            }

            return Ok(cartItem.ToCartItemDto());

        }

        [HttpPut("UpdateUserCartItem/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserProductCart([FromRoute] int productId, [FromBody] UpdateCartItemDto cartItemDto, string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var cartItem = await _shopRepo.UpdateCartAsync(productId, cartItemDto.ToUpdateCartItemDto(), username);
            
            if (cartItem == null)
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound($"Product with {productId} not found");
            }

            _logger.LogInformation("Product updated: {ProductId}", productId);
            return Ok(cartItem.ToCartItemDto());

        }

        [HttpDelete("RemoveMyProduct/{productId:int}")]
      
        public async Task<IActionResult> RemoveMyProductCart([FromRoute,] int productId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);  
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
            _logger.LogError("User not found: {User}", user);
                return Unauthorized();
            }
            var cart = await _shopRepo.RemovefromCartAsync(productId, user);

            if (cart == null)
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound($"Product with {productId} not found");
            }

            _logger.LogInformation("Product removed: {ProductId}", productId);
            return Ok($"Product with {productId} sucessfully removed");
        }



        [HttpDelete("RemoveUserProduct/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserProductCart([FromRoute,] int productId, string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            var cart = await _shopRepo.RemovefromCartAsync(productId, username);

            if (cart == null)
            {
                _logger.LogError("Product not found: {ProductId}", productId);
                return NotFound($"Product with {productId} not found");
            }

         _logger.LogInformation("Product removed: {ProductId}", productId);
            return Ok($"Product with {productId} sucessfully removed");
        }

        [HttpDelete("/ClearMyCart")]
        
        public async Task<IActionResult> ClearCart()
        {
            
            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogError("User not found: {User}", user);
                return Unauthorized();
            }
            await _shopRepo.ClearCartAsync(user);

          _logger.LogInformation("Shopping Carts Cleared");
            return Ok("Shopping Carts Cleared");
        }

        [HttpDelete("/ClearUserCart")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClearCart(string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            await _shopRepo.ClearCartAsync(username);


            _logger.LogInformation("Shopping Carts Cleared");
            return Ok("Shopping Carts Cleared");
        }
        
    }
}
