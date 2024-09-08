using OloEcomm.Dtos.Address;
using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetAddressesAsync();

        Task<Address> GetAddressById(int id);

        Task<Address> CreateAddressAsync (Address address);

        Task<Address> UpdateAddressAsync ( int id, UpdateAddressDto address);

        Task<Address> DeleteAddressAsync (int id);  
    }
}
