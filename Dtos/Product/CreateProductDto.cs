using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OloEcomm.Dtos.Product
{
    public class CreateProductDto
    {
        [MaxLength(20, ErrorMessage ="Name can not be more than 20 characters")] 
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100, ErrorMessage = "Description can not be more than 20 characters")]
        [Required]
        public string Description { get; set; } = string.Empty;


        [Required(ErrorMessage ="Input the Product Quantity")]
        public int QuantityInStock { get; set; } = 2;

        [Column(TypeName = "decimal(18,2)")]
        [Required (ErrorMessage ="Please input the price")]
        public decimal Price { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
       
        [Required(ErrorMessage = "Please input the discount price")]
        public decimal DiscountPrice { get; set; } = decimal.Zero;
    }
}
