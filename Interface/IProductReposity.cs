using OloEcomm.Dtos.Product;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IProductReposity
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product?> GetById (int id);

        Task<Product> CreateProductAsync (Product product); 

        Task<Product?> UpdateProductAsync(int id, Product productModel);

        Task<Product?> DeleteProductAsync (int id);

       Task<bool> productExists (int id);
    }
}
