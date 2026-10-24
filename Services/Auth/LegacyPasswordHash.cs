using System.Security.Cryptography;
using System.Text;

namespace IpDeputyApi.Services.Auth;

public static class LegacyPasswordHash
{
    public static bool Verify(string password, string passwordHash, string passwordSalt)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            var saltBytes = Convert.FromBase64String(passwordSalt);

            var computed = HMACSHA512.HashData(saltBytes, Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(computed, hashBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
