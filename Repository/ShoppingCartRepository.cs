using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Dtos.Cart;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CartItem> AddToCartAsync(int productId, int quantity, string userId, string username, string productAdded)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with ID {productId} does not exist.");
            }


            if (quantity > product.QuantityInStock)
            {
                throw new Exception("Product is not in stock");
            }

            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);


            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                existingCartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = userId,
                    ProductAdded = product.Name, 
                    Quantity = quantity,
                    OrderedBy = username
                };

               
                await _context.CartItems.AddAsync(existingCartItem);
            }

            await _context.SaveChangesAsync();

            return existingCartItem;

        }


        public async Task<CartItem> AddToCartForBuyerAsync(int productId, int quantity, string username)
        {
            
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with ID {productId} does not exist.");
            }



            if (quantity > product.QuantityInStock)
            {
                throw new Exception("Product is not in stock");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new Exception($"User with username {username} does not exist.");
            }

         
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == user.Id);

            if (existingCartItem != null)
            {
                
                existingCartItem.Quantity += quantity;
            }
            else
            {
               
                existingCartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UserId = user.Id
                };

                await _context.CartItems.AddAsync(existingCartItem);
            }

            
            await _context.SaveChangesAsync();

            return existingCartItem;
        }

        public async Task<CartItem> ClearCartAsync(string username)
        {
            var cartItems = _context.CartItems
                .Where(r => r.User.UserName == username);

            

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return null; 
        }


     

        public async Task<List<CartItem>> GetCartsAsync(string username)
        {
            return await _context.CartItems.Include(r => r.User).Where(r => r.User.UserName == username).ToListAsync();
        }

        public async Task<CartItem> GetProductCartIdAsync(int productId, string username)
        {
            var cartItem = await _context.CartItems.Include(r => r.User).Where(r => r.User.UserName == username).FirstOrDefaultAsync(c => c.ProductId == productId);
            if (cartItem == null)
            {
                return null;
            }
            return cartItem;
        }

        public async Task<CartItem> RemovefromCartAsync(int productId, string username)
        {
            var cartItem = await _context.CartItems.Where(r => r.User.UserName == username).FirstOrDefaultAsync(c => c.ProductId == productId);
            if (cartItem == null)
            {
                return null;
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateCartAsync(int productId, CartItem cartItem, string username)
        {
            var existingCartItem = await _context.CartItems.Where(r => r.User.UserName == username).FirstOrDefaultAsync(c => c.ProductId == productId);
            if (existingCartItem == null)
            {
                return null;
            }

            existingCartItem.Quantity = cartItem.Quantity;

            await _context.SaveChangesAsync();
            return existingCartItem;
        }

       
    }
}