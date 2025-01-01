using System.Security.Claims;

namespace OloEcomm.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User is not authenticated");
            }
            var username = user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;

            if (username == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            return username;
        }
    }
}
