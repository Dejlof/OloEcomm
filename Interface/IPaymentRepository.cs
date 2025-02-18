using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentByReference (string reference);

        Task<IEnumerable<Payment>> GetPaymentsByUsers(string username);
    }
}
