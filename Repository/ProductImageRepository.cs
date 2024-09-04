using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Interface;
using OloEcomm.Model;
using static System.Net.Mime.MediaTypeNames;

namespace OloEcomm.Repository
{
    public class ProductImageRepository : IProductImageRepository

    {
        private readonly ApplicationDbContext _context;
        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProductImage> CreateImageAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<ProductImage> DeleteImageAsync(int id)
        {
             var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
            if (image == null)
            {
                return null;
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<List<ProductImage>> GetAllAsync()
        {
            return await _context.ProductImages.ToListAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
            if (image == null)
            {
                return null;
            }

            return image;

        }

        public async Task<ProductImage> UpdateImageAsync(int id, ProductImage image)
        {
            var existingImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
            if (existingImage == null)
            {
                return null;
            }

            existingImage.Url = image.Url;

            await _context.SaveChangesAsync();  
            return existingImage;
        }
    }
}
