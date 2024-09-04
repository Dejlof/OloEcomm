using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.ProductImage
{
    public class UpdateProductImage
    {
        [Required(ErrorMessage = "Image file is required.")]
        public IFormFile? File { get; set; }
    }
}
