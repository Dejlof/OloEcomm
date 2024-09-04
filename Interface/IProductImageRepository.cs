using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetAllAsync();

        Task<ProductImage?> GetByIdAsync(int id);

        Task<ProductImage> CreateImageAsync(ProductImage image);

        Task<ProductImage> UpdateImageAsync(int id, ProductImage image);

        Task<ProductImage> DeleteImageAsync(int id);
    }
}
