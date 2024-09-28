using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IShoppingCartRepository
    {
        Task<List<CartItem>> GetCartsAsync();
        Task<CartItem> AddToCartAsync( int productId, int quantity);
        Task<CartItem> RemovefromCartAsync(int productId);

        Task<CartItem> UpdateCartAsync(int productId, CartItem cartItem);

        Task<CartItem> GetProductCartIdAsync(int productId);

        Task<CartItem> ClearCartAsync();


    }
}
