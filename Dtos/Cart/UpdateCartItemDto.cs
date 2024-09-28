using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Cart
{
    public class UpdateCartItemDto
    {
       
        private int _quantity = 1; 
        [Required]
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value > 0 ? value : 1; } 
        }
    }
}
