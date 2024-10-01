using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "FirstName cannot be blank")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName cannot be blank")]
        public string LastName { get; set; } = string.Empty ;


        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress (ErrorMessage ="Email should be in a proper email format")]
        [Remote(action:"IsEmailAlreadyRegister", controller:"Acccount", ErrorMessage ="Email is already in use")]
        public string Email { get; set; } = string.Empty;

        [Required (ErrorMessage = "Email cannot be blank")]
        [Phone(ErrorMessage = "PhoneNumber should be in a proper PhoneNumber format")]
        [Remote(action: "IsPhoneNumberlAlreadyRegister", controller: "Acccount", ErrorMessage = "PhoneNumber is already in use")]
        public string? PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage ="Password cannot be blank")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ConfirmPassword cannot be blank")]
        [Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        


    }
}
