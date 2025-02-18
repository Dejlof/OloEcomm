using OloEcomm.Dtos.Order;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToOrderDto (this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderedBy = order.OrderedBy,
                AddressOrdered = order.AddressOrdered,
                OrderDetails = order.OrderDetails.Select(s => s.ToOrderDetailsDto()).ToList(),
                Amount = order.Amount,
              
            };
        }
    }
}
