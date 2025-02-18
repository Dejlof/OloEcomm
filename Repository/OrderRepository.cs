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
            return await _context.Orders.Include(s => s.OrderDetails).Where(s => s.OrderedBy == username).ToListAsync();
        }

        
    }
}
