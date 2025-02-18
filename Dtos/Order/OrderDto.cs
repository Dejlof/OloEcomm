using OloEcomm.Dtos.OrderDetails;
using OloEcomm.Model;

namespace OloEcomm.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } 
        public string OrderedBy { get; set; } = string.Empty;
        public string AddressOrdered { get; set; } = string.Empty;
        public ICollection<OrderDetailsDto> OrderDetails { get; set; } 
        public decimal Amount { get; set; }
      
    }
}
