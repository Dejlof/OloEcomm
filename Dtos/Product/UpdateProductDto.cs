using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Product
{
    public class UpdateProductDto
    {

        [MaxLength(20, ErrorMessage = "Name can not be more than 20 characters")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Description can not be more than 100 characters")]
        [Required]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; } = decimal.Zero;
    }
}
