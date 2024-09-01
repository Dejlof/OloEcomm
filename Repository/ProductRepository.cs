﻿using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Dtos.Product;
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

        public async Task<Product> DeleteProductAsync(long id)
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

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(s=>s.ProductImages)
             .Include(s=> s.Reviews).ToListAsync();
        }

        public async Task<Product> GetById(long id)
        {
            var product = await _context.Products.Include(s => s.ProductImages)
             .Include(s => s.Reviews).FirstOrDefaultAsync(x=>x.Id == id);

            if (product == null) 
            {
                return null;
            }
            return product;
        }

        public async Task<Product> UpdateProductAsync(long id, UpdateProductDto productDto)
        {
            var existingProduct = await _context.Products.Include(s => s.ProductImages)
              .Include(s => s.Reviews).FirstOrDefaultAsync(x => x.Id == id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;

            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}