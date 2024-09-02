using OloEcomm.Dtos.Product;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class ProductMappers
    {
        public static ProductDto ToProductDto(this Product productModel)
        {
            return new ProductDto
            {
                Id = productModel.Id,
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                CreatedDate = productModel.CreatedDate,
                CategoryId = productModel.CategoryId,
                Reviews = productModel.Reviews.Select(s => s.ToReviewDto()).ToList()
            };

        }

        public static Product CreateProductDto(this CreateProductDto productDto, int categoryId)
        {
            return new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = categoryId,
            };

        }

        public static Product ToUpdateProductDto(this UpdateProductDto productDto)
        {
            return new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price
            };
       } 
    }
}
