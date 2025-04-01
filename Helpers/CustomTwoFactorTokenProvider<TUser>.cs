using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using OloEcomm.Model;

namespace OloEcomm.Helpers
{
    public class CustomTwoFactorTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser>
    where TUser : IdentityUser
    {

     private static readonly ConcurrentDictionary<string, (string Token, DateTime Expiry)> _tokenStorage = new();
    private readonly IMemoryCache _cache;
    

      public CustomTwoFactorTokenProvider(IMemoryCache cache)
      {
        _cache = cache;
        
      }
      public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
        // 🔹 Allow token generation for all users
        return Task.FromResult(true);
    }

     public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    { 
       
         string cacheKey = $"2FA_{user.Id}_{purpose}";

    // Remove old token if it exists
    _cache.Remove(cacheKey);

    var random = new Random();
    string token = random.Next(100000, 999999).ToString();
    
    _cache.Set(cacheKey, token, TimeSpan.FromMinutes(15));

    return await Task.FromResult(token);
    }
    

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        
           string cacheKey = $"2FA_{user.Id}_{purpose}";

    if (_cache.TryGetValue(cacheKey, out string cachedToken))
    {
        Console.WriteLine($"Validating Token: Expected={cachedToken}, Provided={token}");
        return token == cachedToken;
    }

    Console.WriteLine("Token Not Found in Cache");
    return false;
    }
    }
}