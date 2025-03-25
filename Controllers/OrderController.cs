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
        private readonly ILogger<OrderController> _logger;
        public OrderController(IOrderRepository orderRepository, UserManager<User> userManager,ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _logger = logger;
        }
      
        
        [HttpPost]
        public async Task<IActionResult> CreateOrderFromCart(int addressId)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order request.");   
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
          

            var user = User.GetUsername();
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to order.");
                return Unauthorized();
            }
            var appUser = await _userManager.FindByNameAsync(user);

            try
            {
                _logger.LogInformation("Creating order for user: {User}", user);
                var createdOrder = await _orderRepository.CreateOrderFromCartAsync(appUser.Id, addressId);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder.ToOrderDto());
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Error creating order for user: {User}. Error: {Error}", user, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order request.");   
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with id: {Id} not found.", id);   
                return NotFound();
            }

            var currentUserId = User.GetUsername();
            if (order.OrderedBy != currentUserId)
            {
                _logger.LogWarning("Unauthorized access attempt to order with id: {Id}.", id);
                return Unauthorized("You are not authorized to view this order.");
            }
            _logger.LogInformation("Fetching order with id: {Id}", id);
            return Ok(order.ToOrderDto());
        }

        
        [HttpGet("GetMyOrder")]
        public async Task<IActionResult> GetMyOrder()
        {
            var user = User.GetUsername();
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to order.");
                return Unauthorized();
            }

            var orderModel = await _orderRepository.GetOrdersByUsersAsync(user);
            if (orderModel == null)
            {
                _logger.LogWarning("Order not found for user: {User}.", user);
                return NotFound();
            }
            var orderDto = orderModel.Select(s => s.ToOrderDto()).ToList();
            _logger.LogInformation("Fetching order for user: {User}", user);
            return Ok(orderDto);

        }

        
        [HttpGet("GetOrderbyUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderByUser(string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order request.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var orderModel = await _orderRepository.GetOrdersByUsersAsync(username);
            if (orderModel == null)
            {
                _logger.LogWarning("Order not found for user: {User}.", username);
                return NotFound();
            }
            var orderDto = orderModel.Select(s => s.ToOrderDto()).ToList();
            _logger.LogInformation("Fetching order for user: {User}", username);
            return Ok(orderDto);

        }

      [HttpPost("{orderDetailId}/ShipProductOrdered")]
       [Authorize(Roles="Admin, Vendor")]
        public async Task<IActionResult> ShipOrder(int orderDetailId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Product Ordered request.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Shipping order with id: {Id}", orderDetailId);
                var orderDetail = await _orderRepository.ShipProductOrderAsync(orderDetailId);
                return Ok(orderDetail.ToOrderDetailsDto());
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Error shipping order with id: {Id}. Error: {Error}", orderDetailId, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPost("{orderDetailId}/DeliverProductOrdered")]
        [Authorize(Roles="Admin, Vendor")]
        public async Task<IActionResult> DeliverOrder(int orderDetailId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for productOrdered request.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Delivering order with id: {Id}", orderDetailId);
                var orderDetail = await _orderRepository.DeliverProductOrderAsync(orderDetailId);
                return Ok(orderDetail.ToOrderDetailsDto());
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Error delivering order with id: {Id}. Error: {Error}", orderDetailId, ex.Message);
                return BadRequest(new { message = ex.Message });
            }

    }

    [HttpPost("{orderDetailId}/CancelProductOrdered")]
    [Authorize(Roles="Admin, Vendor")]
    public async Task<IActionResult> CancelOrder(int orderDetailId)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for order request.");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Cancelling order with id: {Id}", orderDetailId);
            var orderDetail = await _orderRepository.CancelProductOrderAsync(orderDetailId);
            return Ok(orderDetail.ToOrderDetailsDto());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Error cancelling order with id: {Id}. Error: {Error}", orderDetailId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }
    }}