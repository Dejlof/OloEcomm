using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Account
{
    public class LoginDto
    {
        [Required]
        public string? Login { get; set; }



        [Required]
        public string? Password { get; set; }
    }
}
