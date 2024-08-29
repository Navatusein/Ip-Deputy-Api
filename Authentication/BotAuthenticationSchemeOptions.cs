using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IpDeputyApi.Authentication
{
    public class BotAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string DefaultSchemeName = "BotAuthenticationScheme";
        public const string TokenHeaderName = "X-BOT-TOKEN";

        public string BotToken { get; set; } = "";
    }
}
