using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Dtos.Address;
using OloEcomm.Interface;
using OloEcomm.Model;

namespace OloEcomm.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;
        public AddressRepository(ApplicationDbContext context) 
        {
        _context = context;
        }
        public async Task<Address> CreateAddressAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> DeleteAddressAsync(int id)
        {
          var address = await _context.Addresses.FirstOrDefaultAsync(s=>s.Id == id);
            if (address == null)
            {
                return null;
            }
            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();
            return address;
           
        }

        public async Task<Address> GetAddressById(int id)
        {
            var address = await _context.Addresses.Include(s=>s.Orders).FirstOrDefaultAsync(s => s.Id == id);
            if (address == null)
            {
                return null;
            }
            return address;
        }

        public async Task<List<Address>> GetAddressesAsync()
        {
           return await _context.Addresses.Include(s=>s.Orders).ToListAsync();
        }

        public async Task<Address> UpdateAddressAsync(int id, UpdateAddressDto address)
        {
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(s=>s.Id==id);
            if (existingAddress == null)
            {
                return null;;
            }
            existingAddress.Street = address.Street;
            existingAddress.City = address.City;
            existingAddress.State = address.State;
            existingAddress.ZipCode = address.ZipCode;
            existingAddress.Country = address.Country;

            await _context.SaveChangesAsync();  
            return existingAddress;
        }
    }
}
