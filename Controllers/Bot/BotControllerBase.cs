using System.Globalization;
using IpDeputyApi.Authentication;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[ApiController]
[Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
public abstract class BotControllerBase : ControllerBase
{
    private static readonly string[] DateFormats = ["yyyy-MM-dd", "dd.MM.yyyy", "dd.MM.yy"];

    protected static DateOnly ParseDate(string? value)
    {
        if (DateOnly.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            return DateOnly.FromDateTime(dateTime);

        throw new ValidationException("dateString", ValidationMessages.InvalidValue);
    }
}
