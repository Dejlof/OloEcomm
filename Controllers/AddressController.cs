using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OloEcomm.Dtos.Address;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;


namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AddressController> _logger;
        public AddressController(IAddressRepository addressRepository, UserManager<User> userManager, ILogger<AddressController> logger)
        {
            _addressRepository = addressRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("GetMyAddresses")]
        public async Task<IActionResult> GetMyAddresses()
        {
            var user = User.GetUsername();
            if (user == null) 
            {
                _logger.LogWarning("Unauthorized access attempt to address.");
                return Unauthorized("User Not Found or UnAuthorized");
            }

           _logger.LogInformation("Fetching address for user: {User}", user);
            var addresses = await _addressRepository.GetAddressesAsync(user);
            var addressesDto = addresses.Select(s => s.ToAddressDto()).ToList();
            return Ok(addressesDto);
        }


        [HttpGet("GetUserAddresses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAddresses(string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request.");
                return BadRequest(ModelState);
            }
            
            _logger.LogInformation("Fetching address for user: {User}", username);
            var addresses = await _addressRepository.GetAddressesAsync(username);
            var addressesDto = addresses.Select(s => s.ToAddressDto()).ToList();
            return Ok(addressesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAddressById( [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request.");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Fetching address with id: {Id}", id);
            var address = await _addressRepository.GetAddressById(id);
            if (address == null)
            {
                _logger.LogWarning("Address not found.");
                return NotFound("Address not found");
            }
            return Ok(address.ToAddressDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress ([FromBody] CreateAddressDto createAddressDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request."); 
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                _logger.LogWarning("User not found.");
                return Unauthorized("User not permitted");
            }

            
            
            var addressModel = createAddressDto.ToCreateAddressDto();
            addressModel.UserId = appUser.Id;
            
            _logger.LogInformation("Creating address for user: {User}", user);

            var address = await _addressRepository.CreateAddressAsync(addressModel);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, addressModel.ToAddressDto());

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress ([FromRoute] int id, [FromBody] UpdateAddressDto updateAddressDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request.");
                return BadRequest(ModelState);
            }

            var address = await _addressRepository.UpdateAddressAsync(id, updateAddressDto);

            if (address == null)
            {
                _logger.LogWarning("Address not found.");
                return NotFound("Address not found");
            }

            var currentUserId = User.GetUsername();
            if (address.UserAddress != currentUserId)
            {
                _logger.LogWarning("Unauthorized access attempt to address.");
                return Unauthorized("You are not authorized to update this address.");
            }
            _logger.LogInformation("Updating address with id: {Id}", id);
            return Ok(address.ToAddressDto());  
        }

        [HttpDelete("DeleteAddress/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id) 
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request.");
                return BadRequest(ModelState);
            }

            var address = await _addressRepository.DeleteAddressAsync(id);
            if (address == null)
            {
                _logger.LogWarning("Address not found.");
                return NotFound("Address not found");
            }
            _logger.LogInformation("Deleting address with id: {Id}", id);
            return Ok("Address sucessfully deleted");
        }

        [HttpDelete("DeleteMyAddress{id:int}")]
        public async Task<IActionResult> DeleteMyAddress([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for address request."); 
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to address.");
                return Unauthorized("User Not UnAuthorized");
            }
            var address = await _addressRepository.DeleteUserAddressAsync(id, user);
            if (address == null)
            {
                _logger.LogWarning("Address not found.");
                return NotFound("Address not found");
            }
            _logger.LogInformation("Deleting address with id: {Id}", id);
            return Ok("Address sucessfully deleted");
        }
    }
}


