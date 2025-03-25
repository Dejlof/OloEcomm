using OloEcomm.Dtos.Product;
using OloEcomm.Helpers;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IProductReposity
    {
        Task<List<Product>> GetAllProductsAsync(ProductQuery productQuery);
        Task<List<Product>> GetUserProductsAsync(string userName);

        Task<Product?> GetById (int id);

        Task<Product> CreateProductAsync (Product product); 

        Task<Product?> UpdateProductAsync(int id, Product productModel);
        Task<Product?> DeleteUserProductAsync(string userName, int id);

        Task<Product?> DeleteProductAsync (int id);

       Task<bool> productExists (int id);

        Task<Product> productExist(int id);

        Task<List<Product>> GetPopularProductsAsync();
    }
}
