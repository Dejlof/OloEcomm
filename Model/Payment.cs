using OloEcomm.Data.Enum;

namespace OloEcomm.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // e.g., Credit Card, PayPal
        public PaymentStatus PaymentStatus { get; set; } // e.g., Paid, Pending
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string Reference { get; set; }

    }
}
