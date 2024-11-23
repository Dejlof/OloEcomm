using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Account
{
    public class LogoutRequestDto
    {
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email format")]
      
        public string Email { get; set; } = string.Empty;
    }
}
