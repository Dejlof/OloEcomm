using OloEcomm.Dtos.Address;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class AddressMapper
    {
        public static AddressDto ToAddressDto(this Address addressModel)
        {
            return new AddressDto
            {
                Id = addressModel.Id,
                Street = addressModel.Street,
                City = addressModel.City,
                State = addressModel.State,
                ZipCode = addressModel.ZipCode,
                Country = addressModel.Country,
                UserAddress= addressModel.User?.UserName
            };
        }

        public static Address ToCreateAddressDto(this CreateAddressDto createAddressDto) 
        {
            return new Address
            {
                Street = createAddressDto.Street,
                City = createAddressDto.City,
                State = createAddressDto.State,
                ZipCode = createAddressDto.ZipCode,
                Country = createAddressDto.Country,
            };
        }

        public static Address ToUpdateAddressDto(this UpdateAddressDto updateAddressDto) 
        {
            return new Address
            {
                Street = updateAddressDto.Street,
                City = updateAddressDto.City,
                State = updateAddressDto.State,
                ZipCode = updateAddressDto.ZipCode,
                Country = updateAddressDto.Country,
            };
        }
    }
}
