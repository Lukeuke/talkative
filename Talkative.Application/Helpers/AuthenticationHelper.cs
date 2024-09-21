using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Talkative.Domain.Entities;
using Talkative.Domain.Models;

namespace Talkative.Application.Helpers;

public static class AuthenticationHelper
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    
    private static byte[] GenerateSalt(int size)
    {
        var salt = new byte[size];
        Rng.GetBytes(salt);
        return salt;
    }

    public static string GenerateHash(string password, string salt)
    {
        var salt1 = Convert.FromBase64String(salt);

#pragma warning disable SYSLIB0041
        using var hashGenerator = new Rfc2898DeriveBytes(password, salt1);
#pragma warning restore SYSLIB0041
        hashGenerator.IterationCount = 10101;
        var bytes = hashGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes);
    }

    public static void ProvideSaltAndHash(this User user)
    {
        var salt = GenerateSalt(24);
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = GenerateHash(user.PasswordHash, user.Salt);
    }
    
    public static string GenerateJwt(ClaimsIdentity subject, Settings settings)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(settings.BearerKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = DateTime.Now.AddSeconds(settings.Expiration),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = settings.Issuer
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        var subject = new ClaimsIdentity(new[]
        {
            // new Claim("AuthType", "JWT"),
            // new Claim("user", user.ToString()),
            new Claim(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Email),
            new Claim("id", user.Id.ToString())
        });
        return subject;
    }

    public static Claim? DeAssembleClaimsIdentity(this string jwt, string claimType)
    {
        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(jwt);

        var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);

        return claim;
    }
}