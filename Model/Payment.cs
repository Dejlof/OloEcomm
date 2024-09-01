using OloEcomm.Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace OloEcomm.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // e.g., Credit Card, PayPal
        public PaymentStatus PaymentStatus { get; set; } // e.g., Paid, Pending
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string Reference { get; set; } = string.Empty;

    }
}
