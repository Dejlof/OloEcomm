using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OloEcomm.Dtos.Account
{
    public class ResetPasswordDto
    {
     [Required(ErrorMessage = "Email cannot be blank")]
     [EmailAddress (ErrorMessage ="Email should be in a proper email format")]
      public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token cannot be blank")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "New Password cannot be blank")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "ConfirmPassword cannot be blank")]
     [Compare("NewPassword", ErrorMessage = "New Password and Confirm password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    }
}