using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Dtos.Address;
using OloEcomm.Interface;
using OloEcomm.Mappers;


namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await _addressRepository.GetAddressesAsync();
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
        public async Task<IActionResult> CreateAddress ([FromBody] CreateAddressDto createAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addressModel = createAddressDto.ToCreateAddressDto();

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
            return Ok(address.ToAddressDto());  
        }

        [HttpDelete("{id:int}")]
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
    }
}


