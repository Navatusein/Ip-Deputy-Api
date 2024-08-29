using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IpDeputyApi.Service
{
    public class JwtService
    {
        private static Serilog.ILogger Logger => Serilog.Log.ForContext<JwtService>();
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public bool ValidateRefreshToken(string token, int studentId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["RefreshJWT:Issuer"],
                ValidAudience = _config["RefreshJWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["RefreshJWT:Key"]!))
            };

            if (!tokenHandler.CanReadToken(token))
                return false;

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (principal.HasClaim(c => c.Type == "StudentId"))
                    return studentId.ToString() == principal.Claims.First(c => c.Type == "StudentId").Value;
            }
            catch (Exception exception)
            {
                Logger.Error("Refresh token validation error: @1", exception);
            }

            return false;
        }

        public string GenerateAuthorizationToken(int studentId, int minutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthorizeJWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("StudentId", studentId.ToString())
            };

            var token = new JwtSecurityToken(
                _config["AuthorizeJWT:Issuer"],
                _config["AuthorizeJWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(int studentId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["RefreshJWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("StudentId", studentId.ToString())
            };

            var token = new JwtSecurityToken(
                _config["RefreshJWT:Issuer"],
                _config["RefreshJWT:Audience"],
                claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
