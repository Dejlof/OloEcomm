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
        public AddressController(IAddressRepository addressRepository, UserManager<User> userManager)
        {
            _addressRepository = addressRepository;
            _userManager = userManager;
        }

        [HttpGet("GetMyAddresses")]
        [Authorize]
        public async Task<IActionResult> GetMyAddresses()
        {
            var user = User.GetUsername();
            if (user == null) 
            {
                return Unauthorized("User Not Found or UnAuthorized");
            }


            var addresses = await _addressRepository.GetAddressesAsync(user);
            var addressesDto = addresses.Select(s => s.ToAddressDto()).ToList();
            return Ok(addressesDto);
        }


        [HttpGet("GetUserAddresses")]
        [Authorize]
        public async Task<IActionResult> GetUserAddresses(string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addresses = await _addressRepository.GetAddressesAsync(username);
            var addressesDto = addresses.Select(s => s.ToAddressDto()).ToList();
            return Ok(addressesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAddressById( [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var address = await _addressRepository.GetAddressById(id);
            if (address == null)
            {
                return NotFound("Address not found");
            }
            return Ok(address.ToAddressDto());

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAddress ([FromBody] CreateAddressDto createAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null)
            {
                return Unauthorized("User not permitted");
            }

            
            
            var addressModel = createAddressDto.ToCreateAddressDto();
            addressModel.UserId = appUser.Id;

            var address = await _addressRepository.CreateAddressAsync(addressModel);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, addressModel.ToAddressDto());

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress ([FromRoute] int id, [FromBody] UpdateAddressDto updateAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _addressRepository.UpdateAddressAsync(id, updateAddressDto);

            if (address == null)
            {
                return NotFound("Address not found");
            }

            var currentUserId = User.GetUsername();
            if (address.UserAddress != currentUserId)
            {
                return Unauthorized("You are not authorized to update this address.");
            }
            return Ok(address.ToAddressDto());  
        }

        [HttpDelete("DeleteAddress/{id:int}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id) 
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _addressRepository.DeleteAddressAsync(id);
            if (address == null)
            {
                return NotFound("Address not found");
            }
            return Ok("Address sucessfully deleted");
        }

        [HttpDelete("DeleteMyAddress{id:int}")]
        public async Task<IActionResult> DeleteMyAddress([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = User.GetUsername();
            if (user == null)
            {
                return Unauthorized("User Not UnAuthorized");
            }
            var address = await _addressRepository.DeleteUserAddressAsync(id, user);
            if (address == null)
            {
                return NotFound("Address not found");
            }
            return Ok("Address sucessfully deleted");
        }
    }
}


