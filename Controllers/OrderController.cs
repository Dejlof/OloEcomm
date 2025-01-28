using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data.Enum;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;
using System.Data;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        public OrderController(IOrderRepository orderRepository, UserManager<User> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrderFromCart(int addressId, [FromQuery] PaymentMethod paymentMethod)
        {

            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
            {
                return BadRequest("Invalid Payment Method selected.");
            }

            var user = User.GetUsername();
            if (user == null)
            {
                return Unauthorized();
            }
            var appUser = await _userManager.FindByNameAsync(user);

            try
            {
                var createdOrder = await _orderRepository.CreateOrderFromCartAsync(appUser.Id, addressId, paymentMethod);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder.ToOrderDto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var currentUserId = User.GetUsername();
            if (order.OrderedBy != currentUserId)
            {
                return Unauthorized("You are not authorized to view this order.");
            }
            return Ok(order.ToOrderDto());
        }

        [Authorize]
        [HttpGet("GetMyOrder")]
        public async Task<IActionResult> GetMyUser()
        {
            var user = User.GetUsername();
            if (user == null)
            {
                return Unauthorized();
            }

            var orderModel = await _orderRepository.GetOrdersByUsersAsync(user);
            if (orderModel == null)
            {
                return NotFound();
            }
            var orderDto = orderModel.Select(s => s.ToOrderDto()).ToList();
            return Ok(orderDto);

        }

        [Authorize]
        [HttpGet("GetOrderbyUser")]
        public async Task<IActionResult> GetOrderByUser(string username)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var orderModel = await _orderRepository.GetOrdersByUsersAsync(username);
            if (orderModel == null)
            {
                return NotFound();
            }
            var orderDto = orderModel.Select(s => s.ToOrderDto()).ToList();
            return Ok(orderDto);

        }

        [Authorize]
        [HttpPut("{orderId:int}/UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromRoute] int orderId, [FromQuery] PaymentStatus paymentStatus)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            if (!Enum.IsDefined(typeof(PaymentStatus), paymentStatus))
            {
                return BadRequest("Invalid Payment Status.");
            }
            try
            {
                var orderPaymentUpdate = await _orderRepository.UpdatePaymentStatus(orderId, paymentStatus);
                return Ok(new { message = "Payment status updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
