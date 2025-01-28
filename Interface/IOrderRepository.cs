using OloEcomm.Data.Enum;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderFromCartAsync(string userId, int addressId, PaymentMethod paymentMethod);

        Task<IEnumerable<Order>> GetOrdersByUsersAsync(string username);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<Order?> UpdatePaymentStatus(int orderId, PaymentStatus paymentStatus);

   }
}
