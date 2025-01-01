using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OloEcomm.Dtos.Account;
using OloEcomm.Interface;
using OloEcomm.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OloEcomm.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _userManager = userManager;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

       
        public async Task<string> CreateToken(User user)
        {
            var claims = new List<Claim>
                {
                  new Claim(JwtRegisteredClaimNames.Email, user.Email),
                   new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                 };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles) 
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
            
      

        }

        public string GenerateRefreshToken(out DateTime expiryTime)
        {
            Byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(bytes);
            expiryTime = DateTime.UtcNow.AddMinutes(60);
            return Convert.ToBase64String(bytes);
        }

 
    }
}
