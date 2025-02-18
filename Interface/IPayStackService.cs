using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IPayStackService
    {
        Task<Payment> InitializePayment( int orderId);
        Task<bool> VerifyPayment(string reference);

    }
}
