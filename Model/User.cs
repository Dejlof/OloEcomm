using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Model
{
    public class User:IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Address> Address { get; set; } = new List<Address>();

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
