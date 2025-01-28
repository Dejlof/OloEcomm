using OloEcomm.Data.Enum;

namespace OloEcomm.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }

        public User? User { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
        public int AddressId { get; set; } 
        public Address Address { get; set; }

        public string AddressOrdered { get; set; } = string.Empty;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public Guid TransactionId { get; set; } = Guid.NewGuid();

    }
}
