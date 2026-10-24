using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Authentication Controller")]
[Route("frontend/authentication")]
public class AuthenticationController : FrontendControllerBase
{
    public const string LoginRateLimitPolicy = "login";

    private const string CookieKey = "RefreshToken";
    private const string CookiePath = "/api/frontend/authentication";

    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthenticationController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(LoginRateLimitPolicy)]
    public async Task<ActionResult<AuthUserDto>> LoginAsync([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        SetRefreshCookie(result.RefreshToken!);
        return Ok(result.User);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthUserDto>> RefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RefreshAsync(Request.Cookies[CookieKey], cancellationToken);
            SetRefreshCookie(result.RefreshToken!);
            return Ok(result.User);
        }
        catch
        {
            ClearRefreshCookie();
            throw;
        }
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public ActionResult Logout()
    {
        ClearRefreshCookie();
        return NoContent();
    }

    [HttpPost("change-password")]
    [EnableRateLimiting(LoginRateLimitPolicy)]
    public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var refreshToken = await _authService.ChangePasswordAsync(User, dto, cancellationToken);
        SetRefreshCookie(refreshToken);
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<AuthUserDto>> GetMeAsync(CancellationToken cancellationToken)
    {
        return Ok(await _authService.GetMeAsync(User, cancellationToken));
    }

    [HttpPost("login-bot")]
    [AllowAnonymous]
    [EnableRateLimiting(LoginRateLimitPolicy)]
    public async Task<ActionResult<AuthUserDto>> LoginBotAsync([FromBody] LoginBotDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LoginBotAsync(dto, cancellationToken));
    }

    private void SetRefreshCookie(string refreshToken)
    {
        Response.Cookies.Delete(CookieKey, new CookieOptions { Path = "/", Secure = true, SameSite = SameSiteMode.None });
        Response.Cookies.Append(CookieKey, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = CookiePath,
            Expires = DateTimeOffset.UtcNow.Add(_tokenService.RefreshTokenLifetime)
        });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Delete(CookieKey, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Path = CookiePath });
        Response.Cookies.Delete(CookieKey, new CookieOptions { Path = "/", Secure = true, SameSite = SameSiteMode.None });
    }
}
