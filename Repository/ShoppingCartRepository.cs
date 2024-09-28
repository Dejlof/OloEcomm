using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
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
        public async Task<CartItem> AddToCartAsync(CartItem cartItem, int productId, int quantity)
        {
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existingCartItem != null) 
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                existingCartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ShoppingCartId = cartItem.ShoppingCartId 
                };

                await _context.CartItems.AddAsync(existingCartItem);
            }

            await _context.SaveChangesAsync();

            return existingCartItem;

        }

        public async  Task<CartItem> ClearCartAsync()
        {
            _context.CartItems.RemoveRange(_context.CartItems);

           
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<List<ShoppingCart>> GetCartsAsync()
        {
            return await _context.ShoppingCarts.Include(s => s.CartItems).ToListAsync();    
        }

        public async Task<CartItem> GetProductCartIdAsync(int id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
            {
                return null;
            }
            return cartItem;
        }

        public async Task<CartItem> RemovefromCartAsync(int id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
            {
                return null;
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
    }
}
