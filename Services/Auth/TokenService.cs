using System.Security.Claims;
using System.Text;
using IpDeputyApi.Authorization;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Options;
using IpDeputyApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IpDeputyApi.Services.Auth;

public class TokenService : ITokenService
{
    private const string RefreshTokenType = "refresh";
    private const string TokenTypeClaim = "typ_use";

    private readonly AccessTokenOptions _accessOptions;
    private readonly RefreshTokenOptions _refreshOptions;
    private readonly IAppClock _clock;
    private readonly JsonWebTokenHandler _handler = new() { MapInboundClaims = false };

    public TokenService(IOptions<AccessTokenOptions> accessOptions, IOptions<RefreshTokenOptions> refreshOptions, IAppClock clock)
    {
        _accessOptions = accessOptions.Value;
        _refreshOptions = refreshOptions.Value;
        _clock = clock;
    }

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_refreshOptions.LifetimeDays);

    public string CreateAdminAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(AppClaims.Subject, user.Id.ToString()),
            new(AppClaims.Kind, UserKinds.Admin),
            new(AppClaims.Name, user.DisplayName),
            new(AppClaims.TokenVersion, user.TokenVersion.ToString())
        };

        if (user.StudentId != null)
            claims.Add(new Claim(AppClaims.StudentId, user.StudentId.Value.ToString()));

        claims.AddRange(user.Permissions.Distinct().Select(x => new Claim(AppClaims.Permission, x)));

        return Create(_accessOptions, claims, TimeSpan.FromMinutes(_accessOptions.LifetimeMinutes));
    }

    public string CreateStudentAccessToken(Student student)
    {
        var claims = new List<Claim>
        {
            new(AppClaims.Subject, student.Id.ToString()),
            new(AppClaims.Kind, UserKinds.Student),
            new(AppClaims.Name, student.Name),
            new(AppClaims.StudentId, student.Id.ToString())
        };

        return Create(_accessOptions, claims, TimeSpan.FromMinutes(_accessOptions.LifetimeMinutes));
    }

    public string CreateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new(AppClaims.Subject, user.Id.ToString()),
            new(AppClaims.TokenVersion, user.TokenVersion.ToString()),
            new(TokenTypeClaim, RefreshTokenType)
        };

        return Create(_refreshOptions, claims, RefreshTokenLifetime);
    }

    public async Task<RefreshTokenData?> ReadRefreshTokenAsync(string token)
    {
        var result = await _handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _refreshOptions.Issuer,
            ValidAudience = _refreshOptions.Audience,
            IssuerSigningKey = CreateKey(_refreshOptions),
            ClockSkew = TimeSpan.FromMinutes(1)
        });

        if (!result.IsValid)
            return null;

        var identity = result.ClaimsIdentity;

        if (identity.FindFirst(TokenTypeClaim)?.Value != RefreshTokenType)
            return null;

        if (!int.TryParse(identity.FindFirst(AppClaims.Subject)?.Value, out var userId))
            return null;

        if (!int.TryParse(identity.FindFirst(AppClaims.TokenVersion)?.Value, out var tokenVersion))
            return null;

        return new RefreshTokenData(userId, tokenVersion);
    }

    private string Create(TokenOptions options, IEnumerable<Claim> claims, TimeSpan lifetime)
    {
        var now = _clock.UtcNow;

        return _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(lifetime),
            SigningCredentials = new SigningCredentials(CreateKey(options), SecurityAlgorithms.HmacSha256)
        });
    }

    private static SymmetricSecurityKey CreateKey(TokenOptions options)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
    }
}
