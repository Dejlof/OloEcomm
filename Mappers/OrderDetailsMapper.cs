using OloEcomm.Dtos.OrderDetails;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class OrderDetailsMapper
    {
        public static OrderDetailsDto ToOrderDetailsDto( this OrderDetail orderDetail)
        {
            return new OrderDetailsDto
            {
                Quantity = orderDetail.Quantity,
                Price = orderDetail.Price,
                TotalPrice = orderDetail.TotalPrice,
                ProductName = orderDetail.ProductName,
            };
        }
       
    }
}
