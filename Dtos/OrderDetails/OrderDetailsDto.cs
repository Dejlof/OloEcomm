using OloEcomm.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace OloEcomm.Dtos.OrderDetails
{
    public class OrderDetailsDto
    {
     
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
       
        public string ProductName { get; set; } = string.Empty;
    }
}
