using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Cart
{
    public class CreateCartItemDto
    {
        [Required]
        public int Quantity { get; set; }
    }
}
