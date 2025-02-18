namespace OloEcomm.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; } 
        public string Reference { get; set; } = string.Empty;

        public string AuthorizationUrl { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;

        public bool Status { get; set; }
        public string Message { get; set; }= string.Empty;


       

        public string PaidBy {  get; set; }=string.Empty;
    }
}
