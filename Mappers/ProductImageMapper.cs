using OloEcomm.Dtos.ProductImage;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class ProductImageMapper
    {
        public static ProductImageDto ToProductImageDto (this ProductImage productImage)
        {
            return new ProductImageDto
            {
                Id = productImage.Id,
                Url = productImage.Url,
                ProductId = productImage.ProductId,
            };
        }

      
    }
}
