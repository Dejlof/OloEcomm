using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Data.Enum;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;


        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<Order> CreateOrderFromCartAsync(string userId, int addressId)
        {
            var cartItems = await _context.CartItems
              .Include(ci => ci.Product)
             .Where(ci => ci.UserId == userId)
              .ToListAsync();

          

            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty. Cannot create order.");
            }

            var totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            var address = await _context.Addresses.Where(ci => ci.UserId == userId).FirstOrDefaultAsync(x=>x.Id == addressId);

            if (address == null)
            {
                throw new InvalidOperationException("Address not found for the given address ID.");
            }

            var order = new Order
            {
                UserId = userId,
                OrderedBy = cartItems.FirstOrDefault()?.User?.UserName,
                AddressId = addressId,
                AddressOrdered = $"{address.Street}, {address.City}, {address.State}, {address.ZipCode}, {address.Country}",
                Amount = totalAmount,
                OrderDate = DateTime.Now,
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    TotalPrice = ci.Quantity * ci.Product.Price,
                    ProductName = ci.Product.Name,  
                }).ToList()
            };

            foreach(var cartItem  in order.OrderDetails){
              cartItem.OrderStatus = OrderStatus.Pending.ToString();
            } 

            await _context.Orders.AddAsync(order);

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.Include(s => s.OrderDetails).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUsersAsync(string username)
        {
            return await _context.Orders.Include(s => s.OrderDetails).Where(s => s.OrderedBy == username).OrderByDescending(s=>s.OrderDate).ToListAsync();
        }

        public async Task<OrderDetail?> ShipProductOrderAsync(int orderDetailId)
        {
            var orderDetail = _context.OrderDetails.Include(o=>o.Shipment).FirstOrDefault(o => o.Id == orderDetailId);
            if (orderDetail == null)
            {
                throw new InvalidOperationException("Product Ordered  not found.");
            }
            if (orderDetail.OrderStatus != OrderStatus.Processing.ToString())
            {
                throw new InvalidOperationException("Order is not in processing state. Cannot ship.");
            } else if (orderDetail.OrderStatus == OrderStatus.Shipped.ToString())
            {
                throw new InvalidOperationException("Product Ordered has already been shipped.");
            }

            var shipment = new Shipment
            {
                OrderDetailId = orderDetailId,
                ShippedDate= DateTime.Now,
            };

            orderDetail.Shipment = shipment;
            orderDetail.Shipment.Id = shipment.Id;
            orderDetail.OrderStatus = OrderStatus.Shipped.ToString();

            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail?> DeliverProductOrderAsync(int orderDetailId)
        {
            var orderDetail = _context.OrderDetails.Include(o=>o.Shipment).FirstOrDefault(o => o.Id == orderDetailId);
            if (orderDetail == null)
            {
                throw new InvalidOperationException("Product Ordered not found.");
            }
            if (orderDetail.OrderStatus != OrderStatus.Shipped.ToString())
            {
                throw new InvalidOperationException("Product Ordered is not in shipped state. Cannot ship.");
            }
            else if (orderDetail.OrderStatus == OrderStatus.Delivered.ToString())
            {
                throw new InvalidOperationException("Product Ordered has already been delivered."); 
            }


            
            
            orderDetail.Shipment.DeliveredDate = DateTime.Now;
            orderDetail.OrderStatus = OrderStatus.Delivered.ToString();

            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail?> CancelProductOrderAsync(int orderDetailId)
        {
            var orderDetail = _context.OrderDetails.Include(o=>o.Shipment).FirstOrDefault(o => o.Id == orderDetailId);
            if (orderDetail == null)
            {
                throw new InvalidOperationException("Product Ordered not found.");
            }

           switch (orderDetail.OrderStatus)
        {
        case nameof(OrderStatus.Pending):
            throw new InvalidOperationException("Product Orderered is in pending state and payment not made. Cannot cancel.");
        case nameof(OrderStatus.Delivered):
            throw new InvalidOperationException("Product Orderered is already delivered. Cannot cancel.");
        case nameof(OrderStatus.Cancelled):
            throw new InvalidOperationException("Product Orderered is already cancelled.");
        }

            orderDetail.OrderStatus = OrderStatus.Cancelled.ToString();

           var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);
                if (product != null)
                {
                    product.QuantityInStock += orderDetail.Quantity;
                }
            

            await _context.SaveChangesAsync();
            return orderDetail;
            
        }

        
    }
}
