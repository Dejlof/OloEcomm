using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IWishlistRepository
    {
        Task<List<Wishlist>> GetAllAsync (string username);

        Task<Wishlist> GetByIdAsync (int id);

        Task<Wishlist> CreateAsync (Wishlist wishlist);

        Task<Wishlist> DeleteAsync(int id, string username);
    }
}
