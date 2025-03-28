﻿using OloEcomm.Data.Enum;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderFromCartAsync(string userId, int addressId);

        Task<IEnumerable<Order>> GetOrdersByUsersAsync(string username);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<OrderDetail?> ShipProductOrderAsync(int orderDetailId);

        Task<OrderDetail?> DeliverProductOrderAsync(int orderDetailId);

        Task<OrderDetail?> CancelProductOrderAsync(int orderDetailId); 
        

   }
}
