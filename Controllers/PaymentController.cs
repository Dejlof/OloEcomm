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
        public PaymentController(IPayStackService payStackService, IPaymentRepository paymentRepository)
        {
            _payStackService = payStackService;
            _paymentRepository = paymentRepository;
        }


        [Authorize]
        [HttpPost("InitializePayment/{orderId:int}")]
        public async Task<IActionResult> InitializePayment([FromRoute] int orderId)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

           
            var payment = await _payStackService.InitializePayment(orderId);
          
            return Ok(payment.ToPaymentDto());

        }

        [HttpGet("Verify/{reference}")]
        public async Task<IActionResult> VerifyPayment([FromRoute] string reference)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var isVerified = await _payStackService.VerifyPayment(reference);
            return isVerified ? Ok("Payment Successful") : BadRequest("Payment Not Sucessful");

        }

        [HttpGet("GetPaymentByReference/{reference}")]
        public async Task<IActionResult> GetPaymentByReference([FromRoute] string reference)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            var payment = await _paymentRepository.GetPaymentByReference(reference);
            if (payment == null)
            {
                return NotFound("Payment Not Found");
            }

            return Ok(payment.ToPaymentDto());

        }

        [HttpGet("GetPaymentsByUserName/{username}")]
        public async Task<IActionResult> GetPaymentsByUserName([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            var payments = await _paymentRepository.GetPaymentsByUsers(username);
            if (payments == null)
            {
                return NotFound("Payments Not Found");
            }
             var paymentsDto = payments.Select(x => x.ToPaymentDto()).ToList();
            return Ok(paymentsDto);
        }
    }
}
