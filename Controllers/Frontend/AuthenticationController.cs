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
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IpDeputyApi.Controllers.Frontend
{
    [Tags("Frontend:Authentication Controller")]
    [Route("frontend/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string CookieKey = "RefreshToken";

        private static Serilog.ILogger Logger => Serilog.Log.ForContext<AuthenticationController>();
        private readonly IpDeputyDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;

        public AuthenticationController(IpDeputyDbContext context, JwtService jwtService, IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
        }

        [HttpPost]
        [Route("login-bot/{initData}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> PostFrontendLoginAsync([FromRoute] string initData, CancellationToken cancellationToken)
        {
            var queryParams = HttpUtility.ParseQueryString(initData);

            if(queryParams == null)
                throw new HttpException("Not authorized student", StatusCodes.Status401Unauthorized);

            var data = queryParams.AllKeys.ToDictionary(key => key!, key => queryParams[key]);


            var checkString = string.Join("\n", data
                .Where(x => x.Key != "hash")
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={x.Value}")
            );

            var secretKey = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"))
                .ComputeHash(Encoding.UTF8.GetBytes(_config["BotToken"]!));

            var signature = new HMACSHA256(secretKey)
                .ComputeHash(Encoding.UTF8.GetBytes(checkString));

            var hexSignature = BitConverter.ToString(signature).Replace("-", "").ToLower();

            if (data["hash"] != hexSignature)
                throw new HttpException("Not authorized student", StatusCodes.Status401Unauthorized);

            var user = JsonConvert.DeserializeObject<Dictionary<string, string>>(data["user"]!);

            var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == ulong.Parse(user!["id"]), cancellationToken);

            if (telegram == null)
                throw new HttpException("Student not found", StatusCodes.Status404NotFound);

            var userDto = new UserDto()
            {
                JwtToken = _jwtService.GenerateAuthorizationToken(telegram.StudentId, false),
                StudentId = telegram.StudentId,
                UserName = telegram.Student.Name
            };

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> PostLoginAsync([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _context.WebAuthentications.FirstOrDefaultAsync(x => x.Login == loginDto.Login.ToLower(), cancellationToken);

            if (user == null || !user.VerifyPasswordHash(loginDto.Password))
                throw new HttpException("Invalid user login or password", StatusCodes.Status400BadRequest);

            var dto = new UserDto()
            {
                JwtToken = _jwtService.GenerateAuthorizationToken(user.StudentId, true),
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
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> PostRefreshAsync([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[CookieKey];

            Logger.Information("Token: {@refreshToken}", refreshToken);

            if (refreshToken == null || !_jwtService.ValidateRefreshToken(refreshToken, userDto.StudentId))
                throw new HttpException("Invalid refresh token", StatusCodes.Status400BadRequest);

            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == userDto.StudentId);

            userDto.JwtToken = _jwtService.GenerateAuthorizationToken(userDto.StudentId, true);

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
        [Route("password/{password}")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, string>> GetGeneratePassword([FromRoute] string password)
        {
            Dictionary<string, string> passwordData = new();

            using var hmac = new HMACSHA512();
            passwordData["Salt"] = Convert.ToBase64String(hmac.Key);
            passwordData["Hash"] = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            return passwordData;
        }
    }
}
