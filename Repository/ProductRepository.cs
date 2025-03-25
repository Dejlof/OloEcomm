using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Dtos.Product;
using OloEcomm.Helpers;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class ProductRepository : IProductReposity
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository( ApplicationDbContext context) 
        { 
        _context = context;
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
             await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();  
            return product;

        }

        public async Task<Product> DeleteProductAsync(int id)
        {
            var productModel = await _context.Products.Include(s => s.ProductImages)
            .Include(s => s.Reviews).FirstOrDefaultAsync(x => x.Id == id);

            if (productModel == null)
            {
                return null;
            }

            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }

        public async Task<Product> DeleteUserProductAsync(string userName, int id)
        {
            var userProductModel = await _context.Products.Include(s => s.ProductImages)
            .Include(s => s.Reviews).Where(s => s.User.UserName == userName).FirstOrDefaultAsync(x => x.Id == id);

            if (userProductModel == null)
            {
                return null;
            }

            _context.Products.Remove(userProductModel);
            await _context.SaveChangesAsync();
            return userProductModel;

        }


        public async Task<List<Product>> GetAllProductsAsync(ProductQuery productQuery)
        {
            var products = _context.Products
     .Include(p => p.ProductImages)
     .Include(p => p.Reviews)
     .Include(p => p.User) 
    .AsQueryable();

        if(!string.IsNullOrEmpty(productQuery.Search))
        {
            products = products.Where(s => s.Name.Contains(productQuery.Search) || s.Description.Contains(productQuery.Search));
        }

        if(productQuery.MinPrice.HasValue)
        {
            products = products.Where(s => s.Price >= productQuery.MinPrice);

        }
        
        
        if(productQuery.MaxPrice.HasValue)
        {
            products = products.Where(s => s.Price <= productQuery.MaxPrice);
        }

        switch (!string.IsNullOrEmpty(productQuery.SortBy) ? productQuery.SortBy.ToLower() : null)
        {
            case "price":
                products = productQuery.IsSortDescending ? products.OrderByDescending(s => s.Price) : products.OrderBy(s => s.Price);
                break;
            case "name":
                products = productQuery.IsSortDescending ? products.OrderByDescending(s => s.Name) : products.OrderBy(s => s.Name);
                break;
            default:
                products = products.OrderByDescending(s => s.CreatedDate);
                break;
        }
        var skipNumber = (productQuery.PageNumber - 1) * productQuery.PageSize;
        return await products.Skip(skipNumber).Take(productQuery.PageSize).ToListAsync();
        }
        
        public async Task<List<Product>> GetUserProductsAsync(string userName)
        {
            return await _context.Products
     .Include(p => p.ProductImages)
     .Include(p => p.Reviews)
     .Include(p => p.User)
     .Where(s => s.User.UserName == userName)
     .ToListAsync();

        }

        public async Task<Product> GetById(int id)
        {
            var product = await _context.Products.Include(s => s.ProductImages)
             .Include(s => s.Reviews).FirstOrDefaultAsync(x=>x.Id == id);

            if (product == null) 
            {
                return null;
            }
            return product;
        }

        public Task<bool> productExists(int id)
        {
            return _context.Products.AnyAsync(x => x.Id == id);
        }

        public async Task<Product> productExist(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> UpdateProductAsync(int id, Product productModel)
        {
            var existingProduct = await _context.Products.Include(s => s.ProductImages)
              .Include(s => s.Reviews).FirstOrDefaultAsync(x => x.Id == id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = productModel.Name;
            existingProduct.Description = productModel.Description;
            existingProduct.Category = productModel.Category;
            existingProduct.QuantityInStock = productModel.QuantityInStock;
            existingProduct.Price = productModel.Price;
            existingProduct.DiscountPrice = productModel.DiscountPrice; 

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>> GetPopularProductsAsync()
        {
            return await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .Include(p => p.User)
            .OrderByDescending(s => s.Reviews.Count)
            .Take(5)
            .ToListAsync();
        }
    }
}
