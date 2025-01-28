using System.Text.Json.Serialization;

namespace OloEcomm.Model
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string UserAddress { get; set; } = string.Empty;
        public string? UserId { get; set; }

      
        public User? User { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>(); 
    }
}
