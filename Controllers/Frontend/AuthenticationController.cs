using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Dtos;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace IpDeputyApi.Controllers.Frontend
{
    [Tags("Frontend Authentication Controller")]
    [Route("frontend/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string CookieKey = "RefreshToken";

        private static Serilog.ILogger Logger => Serilog.Log.ForContext<AuthenticationController>();
        private readonly IpDeputyDbContext _context;
        private readonly JwtService _jwtService;

        public AuthenticationController(IpDeputyDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "Login success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDto), description: "Invalid user login or password")]
        public async Task<ActionResult<UserDto>> PostLoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _context.WebAuthentications.FirstOrDefaultAsync(x => x.Login == loginDto.Login.ToLower(), cancellationToken);

            if (user == null || !user.VerifyPasswordHash(loginDto.Password))
                throw new HttpException("Invalid user login or password", StatusCodes.Status400BadRequest);

            UserDto dto = new UserDto()
            {
                JwtToken = _jwtService.GenerateAuthorizationToken(user.StudentId),
                StudentId = user.StudentId,
                UserName = user.Student.Name
            };

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddDays(30),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            var refreshToken = _jwtService.GenerateRefreshToken(user.StudentId);
            Response.Cookies.Append(CookieKey, refreshToken, cookieOptions);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpPost]
        [Route("login-bot")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "Login success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDto), description: "Not authorized student")]
        public async Task<ActionResult<UserDto>> PostLoginBotAsync(ulong telegramId, CancellationToken cancellationToken)
        {
            var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);

            if (telegram == null)
                throw new HttpException("Not authorized student", StatusCodes.Status400BadRequest);

            var userDto = new UserDto()
            {
                JwtToken = _jwtService.GenerateAuthorizationToken(telegram.StudentId),
                StudentId = telegram.StudentId,
                UserName = telegram.Student.Name
            };

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "Login success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDto), description: "Invalid refresh token")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDto), description: "Invalid user data")]
        public async Task<ActionResult<UserDto>> PostRefreshAsync(UserDto userDto, CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[CookieKey];

            if (refreshToken == null || _jwtService.ValidateRefreshToken(refreshToken, userDto.StudentId))
                throw new HttpException("Invalid refresh token", StatusCodes.Status400BadRequest);

            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == userDto.StudentId);

            userDto.JwtToken = _jwtService.GenerateAuthorizationToken(userDto.StudentId);

            if (student == null)
                throw new HttpException("Invalid user data", StatusCodes.Status400BadRequest);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddDays(30),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            refreshToken = _jwtService.GenerateRefreshToken(userDto.StudentId);
            Response.Cookies.Append(CookieKey, refreshToken, cookieOptions);

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [HttpGet]
        [Route("password")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, string>> GetGeneratePassword(string password)
        {
            Dictionary<string, string> passwordData = new();

            using var hmac = new HMACSHA512();
            passwordData["Salt"] = Convert.ToBase64String(hmac.Key);
            passwordData["Hash"] = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            return passwordData;
        }
    }
}
