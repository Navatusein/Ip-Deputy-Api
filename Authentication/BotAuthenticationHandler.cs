using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace IpDeputyApi.Authentication;

public class BotAuthenticationHandler : AuthenticationHandler<BotAuthenticationSchemeOptions>
{
    public BotAuthenticationHandler(IOptionsMonitor<BotAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrEmpty(Options.BotToken))
            return Task.FromResult(AuthenticateResult.Fail("Bot token is not configured"));

        if (!Request.Headers.TryGetValue(BotAuthenticationSchemeOptions.TokenHeaderName, out var header))
            return Task.FromResult(AuthenticateResult.NoResult());

        var provided = Encoding.UTF8.GetBytes(header.ToString());
        var expected = Encoding.UTF8.GetBytes(Options.BotToken);

        if (!CryptographicOperations.FixedTimeEquals(provided, expected))
            return Task.FromResult(AuthenticateResult.Fail("Incorrect bot token"));

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "Bot")], Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
