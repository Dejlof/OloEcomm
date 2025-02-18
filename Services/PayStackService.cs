using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using OloEcomm.Data;
using OloEcomm.Data.Enum;
using OloEcomm.Interface;
using OloEcomm.Model;
using OloEcomm.Model.Paysatck;
using PayStack.Net;
using System.Net.Http.Headers;
using System.Text;

namespace OloEcomm.Services
{
    public class PayStackService : IPayStackService
    {
        private readonly string _secretKey;
        private readonly PayStackApi _payStack;
        private readonly ApplicationDbContext _dbContext;
        public PayStackService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _secretKey = configuration["Paystack:SecretKey"];
            _payStack = new PayStackApi(_secretKey);
            _dbContext = dbContext;
        }
        public async Task<Payment> InitializePayment(int orderId)
        {

            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            var amount = order.Amount;
            var email = order.OrderedBy;
            var reference = Guid.NewGuid().ToString();

            

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestBody = new
                {
                    email = email,
                    amount = (int)(amount * 100),  // Convert amount to kobo
                    reference = reference,
                    callback_url = "https://your-site.com/payment-success"
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");


                Console.WriteLine($"Request Headers: {httpClient.DefaultRequestHeaders}");
                Console.WriteLine($"Request Body: {JsonConvert.SerializeObject(requestBody)}");

                var response = await httpClient.PostAsync("https://api.paystack.co/transaction/initialize", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Paystack Response: {responseString}");
                var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(responseString);

                if (paystackResponse.Status)
                {
                    var payment = new Payment
                    {
                        Amount = amount,
                        PaymentDate = paystackResponse.Data.PaidAt,
                        PaymentStatus = PaymentStatus.Pending.ToString(),
                        Reference =paystackResponse.Data.Reference,
                        OrderId = orderId,
                        PaidBy = email,
                        AuthorizationUrl = paystackResponse.Data.AuthorizationUrl,
                        AccessCode = paystackResponse.Data.AccessCode,
                        Message = paystackResponse.Message,
                        Status = paystackResponse.Status,
                    };

                    await _dbContext.Payments.AddAsync(payment);
                    await _dbContext.SaveChangesAsync();

                    return payment;
                }

                throw new Exception($"Payment initialization failed. Paystack response: {paystackResponse.Message}");
            }
        }

        public async Task<bool> VerifyPayment(string reference)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_secretKey);
                var response = await httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Paystack Response: {responseString}");
                var paystackResponse = JsonConvert.DeserializeObject<PaystackVerifyResponse>(responseString);

                var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.Reference == reference);

                if (payment != null)
                {
                    if (paystackResponse.Status && paystackResponse.Data.Status == "success")
                    {
                        payment.PaymentStatus = PaymentStatus.Completed.ToString();
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                    payment.PaymentStatus = PaymentStatus.Failed.ToString();
                    await _dbContext.SaveChangesAsync();
                    return false;
                } 
            }
            return false;
        }
    }
}
