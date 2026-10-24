using System.Security.Claims;
using System.Text;
using IpDeputyApi.Authorization;
using IpDeputyApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IpDeputyApi.Services;

public class JwtService
{
    private readonly AccessTokenOptions _accessOptions;
    private readonly RefreshTokenOptions _refreshOptions;

    public JwtService(IOptions<AccessTokenOptions> accessOptions, IOptions<RefreshTokenOptions> refreshOptions)
    {
        _accessOptions = accessOptions.Value;
        _refreshOptions = refreshOptions.Value;
    }

    public bool ValidateRefreshToken(string token, int studentId)
    {
        var handler = new JsonWebTokenHandler { MapInboundClaims = false };

        var parameters = new TokenValidationParameters
        {
            ValidIssuer = _refreshOptions.Issuer,
            ValidAudience = _refreshOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshOptions.Key))
        };

        var result = handler.ValidateTokenAsync(token, parameters).GetAwaiter().GetResult();
        return result.IsValid && result.ClaimsIdentity.FindFirst("StudentId")?.Value == studentId.ToString();
    }

    public string GenerateAuthorizationToken(int studentId, bool admin)
    {
        var claims = new List<Claim>
        {
            new(AppClaims.Subject, studentId.ToString()),
            new(AppClaims.StudentId, studentId.ToString()),
            new(AppClaims.Kind, admin ? UserKinds.Admin : UserKinds.Student)
        };

        if (admin)
            claims.AddRange(Permissions.AllKeys.Select(x => new Claim(AppClaims.Permission, x)));

        return Write(_accessOptions, claims, DateTime.UtcNow.AddMinutes(_accessOptions.LifetimeMinutes));
    }

    public string GenerateRefreshToken(int studentId)
    {
        return Write(_refreshOptions, [new Claim("StudentId", studentId.ToString())], DateTime.UtcNow.AddDays(_refreshOptions.LifetimeDays));
    }

    private static string Write(TokenOptions options, IEnumerable<Claim> claims, DateTime expires)
    {
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)), SecurityAlgorithms.HmacSha256)
        });
    }
}
