namespace OloEcomm.Model.Paysatck
{
    public class PaystackVerifyData
    {
        public string Status { get; set; }  // Expected values: "success", "failed"
        public string Reference { get; set; }
        public string GatewayResponse { get; set; }
        public int Amount { get; set; } // Amount in kobo (smallest currency unit)
        public string Currency { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
