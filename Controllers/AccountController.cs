using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Account;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Model;
using System.ComponentModel.DataAnnotations;
using static OloEcomm.Dtos.Account.RefreshTokenRequestDo;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext   _context;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ITokenService tokenService, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("Get-all-users")]
        public async Task <IActionResult> GetAll() 
        {
       
          _logger.LogInformation("Fetching All Users on the platform");
            var users = await _userManager.Users.Select(u => new UserDto {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName,
                Role = u.Role,
            }).ToListAsync();

            return Ok(users);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto, [FromQuery] Role role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                     _logger.LogWarning("Invalid model state for Register model.");
                    string errorMessage = string.Join("|",
                        ModelState.Values.SelectMany(x => x.Errors).Select(
                            e => e.ErrorMessage));
                    throw new ArgumentException(errorMessage);
                    
                }

                if (!Enum.IsDefined(typeof(Role), role))
                {
                    _logger.LogWarning("Invalid role selected.");
                    return BadRequest("Invalid role selected.");
                }

                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Role = role.ToString(),
                };
                _logger.LogInformation("Creating new user: {User}", user);
                IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully: {User}", user);
                    IdentityResult assignRole;

                
                    if (role == Role.Vendor)
                    {
                        _logger.LogInformation("Assigning Vendor role to user: {User}", user);
                        assignRole = await _userManager.AddToRoleAsync(user, "Vendor");
                    }
                    else if (role == Role.Buyer)
                    {
                        _logger.LogInformation("Assigning Buyer role to user: {User}", user);
                        assignRole = await _userManager.AddToRoleAsync(user, "Buyer");
                    }
                    else
                    {
                            _logger.LogWarning("Invalid role specified.");
                        throw new ArgumentException("Invalid role specified.");
                    }
                    var refreshToken = _tokenService.GenerateRefreshToken(out DateTime expiryTime);
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = expiryTime;


                    await _userManager.UpdateAsync(user);

                    if (assignRole.Succeeded)
                    {
                        _logger.LogInformation("Role assigned successfully: {User}", user);
                        _logger.LogInformation("Generating token for user: {User}", user);
                    
                        return Ok(
                            new NewUserDto
                        {
                            Email = registerDto.Email,
                            PhoneNumber = registerDto.PhoneNumber,
                            Role = role.ToString(),
                            Token = await _tokenService.CreateToken(user),
                            RefreshToken = refreshToken,
                            RefreshTokenExpiryTime = expiryTime,
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Role assignment failed: {User}", user);
                        var errorMessages = string.Join(", ", assignRole.Errors.Select(e => e.Description));
                        return StatusCode(500, $"Role assignment failed: {errorMessages}");
                    }

                }
                else
                {
                    _logger.LogWarning("User creation failed: {User}", user);
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                    return StatusCode(500, $"Role assignment failed: {errorMessages}");
                }
            }
            catch (Exception ex) 
            { 
                _logger.LogError("An error occurred: {Error}", ex.Message);
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Login model.");
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

          _logger.LogInformation("Fetching user: {User}", loginDto.Login);
            var user = await _userManager.FindByNameAsync(loginDto.Login);

            if (user == null && new EmailAddressAttribute().IsValid(loginDto.Login))
            {
                _logger.LogInformation("Fetching user by email: {User}", loginDto.Login);
                user = await _userManager.FindByEmailAsync(loginDto.Login);
            }

            if (user == null && new PhoneAttribute().IsValid(loginDto.Login))
            {
                _logger.LogInformation("Fetching user by phone: {User}", loginDto.Login);
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.Login);
            }

            if (user == null)
            {
                _logger.LogWarning("User not found: {User}", loginDto.Login);
                return NotFound("User not found");
            }

           _logger.LogInformation("Verifying user: {User}", user);
            var verifiedUser = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!verifiedUser.Succeeded)
            {
                _logger.LogWarning("Invalid credentials for user: {User}", user);
                return Unauthorized("Username/Password incorrect");
            }

            _logger.LogInformation("Generating token for user: {User}", user);
            var accessToken = await _tokenService.CreateToken(user);

            var refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(60);

            var refreshToken =  _tokenService.GenerateRefreshToken(out refreshTokenExpiryTime);
    
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User logged in successfully: {User}", user);

            return Ok(new 
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiryTime = refreshTokenExpiryTime
            });
            
        }

        [HttpPost("GenerateNewRefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for RefreshToken model.");  
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
            
            var user = await _userManager.FindByEmailAsync(request.Email);

          
            if (user == null || user.RefreshToken != request.RefreshToken)
            {
                _logger.LogWarning("Invalid refresh token.");
                return Unauthorized("Invalid refresh token.");
            }

            
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token has expired.");
                return Unauthorized("Refresh token has expired.");
            }

            _logger.LogInformation("Generating new token for user: {User}", user);
            var newAccessToken = await _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(out DateTime newExpiry);

          
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newExpiry;
            await _userManager.UpdateAsync(user);
          _logger.LogInformation("New token generated for user: {User}", user);
          
            return Ok(new
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = newExpiry
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for Logout model.");
                    string errorMessage = string.Join("|",
                        ModelState.Values.SelectMany(x => x.Errors).Select(
                            e => e.ErrorMessage));
                    throw new ArgumentException(errorMessage);
                }

              
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Invalid user.");
                    return BadRequest("Invalid user.");
                }

                // Invalidate the refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User logged out successfully: {User}", user);

                return Ok("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Error}", ex.Message);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        [HttpGet("GetByUser")]
        public async Task<IActionResult> Get(string email)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Get model.");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User not found: {User}", email);
                return NotFound("User not found");
            }

            var userDto = new UserDto
            {

                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

         _logger.LogInformation("Fetching user: {User}", user);
            return Ok(userDto);
        }

        [HttpDelete("Deleteuser")]
        public async Task<IActionResult> Delete(string email)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Delete model.");    
                string errorMessage = string.Join("|",
                    ModelState.Values.SelectMany(x => x.Errors).Select(
                        e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            var deletedUser = await _userManager.FindByNameAsync(email);

            if (deletedUser == null)
            {
                _logger.LogWarning("User not found: {User}", email);
                return NotFound("User not found");
            }

            await _userManager.DeleteAsync(deletedUser);

            _logger.LogInformation("User deleted successfully: {User}", deletedUser);

            return Ok($"{deletedUser} has been deleted");

        }

        [HttpPost("SwitchRoles")]
        public async Task<IActionResult> SwitchRoles([FromQuery] Role role)
        {
             if (!ModelState.IsValid)
         {
        _logger.LogWarning("Invalid model state for SwitchRoles.");
        string errorMessage = string.Join("|", ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
        return BadRequest(errorMessage); // Return BadRequest instead of throwing an exception
          }

              var getUser = User.GetUsername();
           var appUser = await _userManager.FindByNameAsync(getUser);

              if (appUser == null)
            {
        _logger.LogWarning("User not found: {User}", getUser);
        return NotFound("User not found");
              }

            try
          {
        
        var currentRoles = await _userManager.GetRolesAsync(appUser);

        await _userManager.RemoveFromRolesAsync(appUser, currentRoles);

        await _userManager.AddToRoleAsync(appUser, role.ToString());
            appUser.Role = role.ToString();
       
        await _userManager.UpdateAsync(appUser);

     


        _logger.LogInformation("User role updated successfully: {User}", appUser.UserName);
        return Ok($"User {appUser.UserName} role has been updated to {role}");
            }
           catch (Exception ex)
         {
        _logger.LogError("An error occurred while switching roles: {Error}", ex.Message);
        return StatusCode(500, "An error occurred while updating the role.");
           }
         }

        }
    }
