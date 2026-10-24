using IpDeputyApi.Database.Models;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IpDeputyApi.Services.Auth;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(User user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    public PasswordCheckResult Verify(User user, string password)
    {
        if (user.LegacyPasswordSalt != null)
        {
            return LegacyPasswordHash.Verify(password, user.PasswordHash, user.LegacyPasswordSalt)
                ? PasswordCheckResult.SuccessRehashNeeded
                : PasswordCheckResult.Failed;
        }

        try
        {
            return _hasher.VerifyHashedPassword(user, user.PasswordHash, password) switch
            {
                PasswordVerificationResult.Success => PasswordCheckResult.Success,
                PasswordVerificationResult.SuccessRehashNeeded => PasswordCheckResult.SuccessRehashNeeded,
                _ => PasswordCheckResult.Failed
            };
        }
        catch (FormatException)
        {
            return PasswordCheckResult.Failed;
        }
    }
}
