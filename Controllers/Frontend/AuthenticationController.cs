using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IpDeputyApi.Database;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Options;
using IpDeputyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IpDeputyApi.Controllers.Frontend;

public class LegacyLoginDto
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LegacyUserDto
{
    public int StudentId { get; set; }
    public string UserName { get; set; } = null!;
    public string JwtToken { get; set; } = null!;
}

[Tags("Frontend:Authentication Controller")]
[Route("frontend/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private const string CookieKey = "RefreshToken";

    private readonly IpDeputyDbContext _context;
    private readonly JwtService _jwtService;
    private readonly BotOptions _botOptions;

    public AuthenticationController(IpDeputyDbContext context, JwtService jwtService, IOptions<BotOptions> botOptions)
    {
        _context = context;
        _jwtService = jwtService;
        _botOptions = botOptions.Value;
    }

    [HttpPost("login-bot/{initData}")]
    [AllowAnonymous]
    public async Task<ActionResult<LegacyUserDto>> PostFrontendLoginAsync([FromRoute] string initData, CancellationToken cancellationToken)
    {
        var data = QueryHelpers.ParseQuery(Encoding.UTF8.GetString(Convert.FromBase64String(initData)))
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        if (!data.TryGetValue("hash", out var hash))
            throw new UnauthorizedException();

        var checkString = string.Join("\n", data.Where(x => x.Key != "hash").OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{x.Key}={x.Value}"));
        var secretKey = HMACSHA256.HashData(Encoding.UTF8.GetBytes("WebAppData"), Encoding.UTF8.GetBytes(_botOptions.BotToken));
        var signature = Convert.ToHexStringLower(HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(checkString)));

        if (signature != hash)
            throw new UnauthorizedException();

        var userId = JsonDocument.Parse(data["user"]).RootElement.GetProperty("id").GetUInt64();
        var telegram = await _context.Telegrams.Include(x => x.Student).FirstOrDefaultAsync(x => x.TelegramId == userId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");

        return Ok(new LegacyUserDto
        {
            JwtToken = _jwtService.GenerateAuthorizationToken(telegram.StudentId, false),
            StudentId = telegram.StudentId,
            UserName = telegram.Student.Name
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LegacyUserDto>> PostLoginAsync([FromBody] LegacyLoginDto loginDto, CancellationToken cancellationToken)
    {
        var login = loginDto.Login.ToLower();
        var user = await _context.WebAuthentications.Include(x => x.Student).FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

        if (user == null || !LegacyPasswordHash.Verify(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            throw new HttpException("Невірний логін або пароль", StatusCodes.Status400BadRequest);

        Response.Cookies.Append(CookieKey, _jwtService.GenerateRefreshToken(user.StudentId), CookieOptions());

        return Ok(new LegacyUserDto
        {
            JwtToken = _jwtService.GenerateAuthorizationToken(user.StudentId, true),
            StudentId = user.StudentId,
            UserName = user.Student.Name
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LegacyUserDto>> PostRefreshAsync([FromBody] LegacyUserDto userDto, CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[CookieKey];

        if (refreshToken == null || !_jwtService.ValidateRefreshToken(refreshToken, userDto.StudentId))
            throw new UnauthorizedException("Недійсний токен оновлення");

        if (!await _context.WebAuthentications.AnyAsync(x => x.StudentId == userDto.StudentId, cancellationToken))
            throw new UnauthorizedException("Недійсний токен оновлення");

        userDto.JwtToken = _jwtService.GenerateAuthorizationToken(userDto.StudentId, true);
        Response.Cookies.Append(CookieKey, _jwtService.GenerateRefreshToken(userDto.StudentId), CookieOptions());

        return Ok(userDto);
    }

    private static CookieOptions CookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Secure = true,
            SameSite = SameSiteMode.None
        };
    }
}
