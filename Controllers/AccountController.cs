﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Account;
using OloEcomm.Interface;
using OloEcomm.Model;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto, [FromQuery] Role role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string errorMessage = string.Join("|",
                        ModelState.Values.SelectMany(x => x.Errors).Select(
                            e => e.ErrorMessage));
                    throw new ArgumentException(errorMessage);
                }

                if (!Enum.IsDefined(typeof(Role), role))
                {
                    return BadRequest("Invalid role selected.");
                }

                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                };

                IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {

                    IdentityResult assignRole;

                
                    if (role == Role.Vendor)
                    {
                        assignRole = await _userManager.AddToRoleAsync(user, "Vendor");
                    }
                    else if (role == Role.Buyer)
                    {
                        assignRole = await _userManager.AddToRoleAsync(user, "Buyer");
                    }
                    else
                    {
                        throw new ArgumentException("Invalid role specified.");
                    }

                    if (assignRole.Succeeded)
                    {
                        return Ok($"User created with {role} selected");
                    }
                    else
                    {
                        var errorMessages = string.Join(", ", assignRole.Errors.Select(e => e.Description));
                        return StatusCode(500, $"Role assignment failed: {errorMessages}");
                    }

                }
                else
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                    return StatusCode(500, $"Role assignment failed: {errorMessages}");
                }
            }
            catch (Exception ex) 
            { 
             return StatusCode (500, ex.Message);
            }
        }

        [HttpGet("EmailAlreadyRegistered")]
        public async Task<IActionResult> IsEmailAlreadyRegister (string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }



        [HttpGet("PhoneRegistered")]
        public async Task<IActionResult> IsPhoneAlreadyRegister(string phone)
        {
            User? user = await _userManager.Users.FirstOrDefaultAsync(u=> u.PhoneNumber == phone);

            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginDto.Login);

            if (user == null && new EmailAddressAttribute().IsValid(loginDto.Login))
            {
                user = await _userManager.FindByEmailAsync(loginDto.Login);
            }

            if (user == null && new PhoneAttribute().IsValid(loginDto.Login))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.Login);
            }

            if (user == null)
            {
                return NotFound("User not found");
            }

            var verifiedUser = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!verifiedUser.Succeeded)
            {
                return Unauthorized("Username/Password incorrect");
            }

            var Token = await _tokenService.CreateToken(user);

            return Ok(Token);
        }
    }
}