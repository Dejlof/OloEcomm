using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data.Enum;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IPayStackService _payStackService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPayStackService payStackService, IPaymentRepository paymentRepository, ILogger<PaymentController> logger)
        {
            _payStackService = payStackService;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }


        [Authorize]
        [HttpPost("InitializePayment/{orderId:int}")]
        public async Task<IActionResult> InitializePayment([FromRoute] int orderId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for payment request.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var username = User.GetUsername();

           
            var payment = await _payStackService.InitializePayment(orderId,username);
            if (payment == null)
            {
                _logger.LogError("Error initializing payment for user: {User}", username);
                return BadRequest("Payment Not Sucessful");
            }
            _logger.LogInformation("Payment initialized for user: {User}", username);
            return Ok(payment.ToPaymentDto());

        }

        [HttpGet("Verify/{reference}")]
        public async Task<IActionResult> VerifyPayment([FromRoute] string reference)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for payment verification request.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var isVerified = await _payStackService.VerifyPayment(reference);
            if (!isVerified)
            {
                _logger.LogError("Error verifying payment with reference: {Reference}", reference);
                return BadRequest("Payment Not Sucessful");
            }
            
            _logger.LogInformation("Payment verified with reference: {Reference}", reference);
            return isVerified ? Ok("Payment Successful") : BadRequest("Payment Not Sucessful");

        }

        [HttpGet("GetPaymentByReference/{reference}")]
        public async Task<IActionResult> GetPaymentByReference([FromRoute] string reference)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for payment request.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            var payment = await _paymentRepository.GetPaymentByReference(reference);
            if (payment == null)
            {
                _logger.LogWarning("Payment with reference: {Reference} not found.", reference);
                return NotFound("Payment Not Found");
            }

             _logger.LogInformation("Fetching payment with reference: {Reference}", reference);
            return Ok(payment.ToPaymentDto());

        }

        [HttpGet("GetPaymentsByUserName/{username}")]
        [Authorize(Roles= "Admin")]
        public async Task<IActionResult> GetPaymentsByUserName([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for payment request.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            var payments = await _paymentRepository.GetPaymentsByUsers(username);
            if (payments == null)
            {
                _logger.LogWarning("Payments not found for user: {User}.", username);
                return NotFound("Payments Not Found");
            }

            _logger.LogInformation("Fetching payments for user: {User}", username);
             var paymentsDto = payments.Select(x => x.ToPaymentDto()).ToList();
            return Ok(paymentsDto);
        }
    }
}
