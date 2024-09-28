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
        public async Task<CartItem> AddToCartAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with ID {productId} does not exist.");
            }

            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                existingCartItem = new CartItem
                {
                    ProductId = productId,


                };

                await _context.CartItems.AddAsync(existingCartItem);
            }

            await _context.SaveChangesAsync();

            return existingCartItem;

        }

        public async Task<CartItem> ClearCartAsync()
        {
            _context.CartItems.RemoveRange(_context.CartItems);


            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<List<CartItem>> GetCartsAsync()
        {
            return await _context.CartItems.ToListAsync();
        }

        public async Task<CartItem> GetProductCartIdAsync(int productId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId);
            if (cartItem == null)
            {
                return null;
            }
            return cartItem;
        }

        public async Task<CartItem> RemovefromCartAsync(int productId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId);
            if (cartItem == null)
            {
                return null;
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateCartAsync(int productId, CartItem cartItem)
        {
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId);
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