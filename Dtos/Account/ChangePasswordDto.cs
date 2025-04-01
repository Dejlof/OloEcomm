using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OloEcomm.Dtos.Account
{
    public class ChangePasswordDto
    {
    [Required(ErrorMessage = "Current Password cannot be blank")]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "New Password cannot be blank")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "ConfirmPassword cannot be blank")]
    [Compare("NewPassword", ErrorMessage = "NewPassword and Confirm password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
    }
}