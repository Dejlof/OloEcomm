using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;
        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Wishlist> CreateAsync(Wishlist wishlist)
        {
           await _context.AddAsync(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task<Wishlist> DeleteAsync(int id, string username)
        {
           var wishlist = await _context.Wishlist.Where(s=>s.User.UserName==username).FirstOrDefaultAsync(x => x.Id == id);

            if (wishlist == null)
            {
                return null;
            }

            _context.Wishlist.Remove(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task<List<Wishlist>> GetAllAsync(string username)
        {
            return await _context.Wishlist.Include(r => r.User).Where(s => s.User.UserName == username).ToListAsync();
        }

        public async Task<Wishlist> GetByIdAsync(int id)
        {
            var wishlistModel = await _context.Wishlist.Include(r => r.User).FirstOrDefaultAsync(x=>x.Id==id); 

            if (wishlistModel == null)
            {
                return null;
            }
            return wishlistModel;
        }
    }
}
