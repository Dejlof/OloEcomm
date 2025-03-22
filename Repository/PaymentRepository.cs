using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Payment> GetPaymentByReference(string reference)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x=>x.Reference == reference);
            if (payment == null) 
            {
                return null;
            }
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUsers(string username)
        {
            var paymentForUser = await _context.Payments.Where(s=> s.PaidBy == username).OrderByDescending(s=>s.PaymentStatus).ToListAsync();
            if (paymentForUser == null) 
            {
                return null;
            }
            return paymentForUser;
        }
    }
}
