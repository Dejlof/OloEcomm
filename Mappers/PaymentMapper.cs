using OloEcomm.Dtos.Payment;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentDto ToPaymentDto(this Payment paymentModel)
        {
            return new PaymentDto
            {
               
                OrderId = paymentModel.OrderId,
                Amount = paymentModel.Amount,
                PaymentDate = paymentModel.PaymentDate,
                PaymentStatus = paymentModel.PaymentStatus,
                Reference = paymentModel.Reference,
                PaidBy = paymentModel.PaidBy,
                AuthorizationUrl = paymentModel.AuthorizationUrl,
                AccessCode = paymentModel.AccessCode,
                Status = paymentModel.Status,
                Message = paymentModel.Message,
            };
        }
    }
}
