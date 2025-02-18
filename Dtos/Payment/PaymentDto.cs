﻿namespace OloEcomm.Dtos.Payment
{
    public class PaymentDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; } 
        public string Reference { get; set; } = string.Empty;

        public string PaidBy { get; set; } = string.Empty;

        public string AuthorizationUrl { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;

        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;


       
    }
}
