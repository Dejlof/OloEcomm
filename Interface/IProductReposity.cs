using OloEcomm.Dtos.Product;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IProductReposity
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product> GetById (long id);

        Task<Product> CreateProductAsync (Product product); 

        Task<Product> UpdateProductAsync(long id, UpdateProductDto productDto);

        Task<Product> DeleteProductAsync (long id);
    }
}
