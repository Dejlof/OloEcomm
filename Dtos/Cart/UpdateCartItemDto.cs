using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Cart
{
    public class UpdateCartItemDto
    {
        [Required]
        public int Quantity { get; set; }
    }
}
