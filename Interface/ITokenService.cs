using OloEcomm.Model;
using System.Security.Claims;

namespace OloEcomm.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);

        string GenerateRefreshToken(out DateTime expiryTime);

       
    }
}
