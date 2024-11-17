using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface ITokenService
    {
        Task <string> CreateToken (User user);
    }
}
