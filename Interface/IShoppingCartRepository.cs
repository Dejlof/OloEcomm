using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IShoppingCartRepository
    {
       
        Task<List<CartItem>> GetCartsAsync(string username);
        Task<CartItem> AddToCartAsync( int productId, int quantity, string userId, string username, string productAdded);
        Task<CartItem> AddToCartForBuyerAsync(int productId, int quantity, string username);
        
        Task<CartItem> RemovefromCartAsync(int productId, string username);

        Task<CartItem> UpdateCartAsync(int productId, CartItem cartItem, string username);

       
        Task<CartItem> GetProductCartIdAsync(int productId, string username);

        Task<CartItem> ClearCartAsync(string username);


    }
}
