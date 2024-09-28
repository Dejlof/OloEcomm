using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IShoppingCartRepository
    {
        Task<List<ShoppingCart>> GetCartsAsync();
        Task<CartItem> AddToCartAsync(CartItem cartItem, int productId, int quantity);
        Task<CartItem> RemovefromCartAsync(int id);

        Task<CartItem> GetProductCartIdAsync(int id);

        Task<CartItem> ClearCartAsync();


    }
}
