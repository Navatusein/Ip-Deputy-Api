using IpDeputyApi.Dto;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IpDeputyApi.Extensions;

public static class ModelStateExtensions
{
    public static IActionResult ToValidationResult(this ActionContext context)
    {
        var errors = context.ModelState
            .Where(x => x.Value is { Errors.Count: > 0 })
            .GroupBy(x => NormalizeKey(x.Key))
            .ToDictionary(
                x => x.Key,
                x => x.SelectMany(e => e.Value!.Errors).Select(TranslateError).Distinct().ToArray());

        return new BadRequestObjectResult(new ErrorDto(StatusCodes.Status400BadRequest, ValidationException.DefaultMessage, errors));
    }

    private static string NormalizeKey(string key)
    {
        var normalized = key.StartsWith("$.") ? key[2..] : key.TrimStart('$');

        if (normalized.Length == 0)
            return "body";

        return string.Join('.', normalized.Split('.').Select(part => part.Length == 0 ? part : char.ToLowerInvariant(part[0]) + part[1..]));
    }

    private static string TranslateError(ModelError error)
    {
        if (error.Exception != null)
            return ValidationMessages.InvalidValue;

        var message = error.ErrorMessage;

        if (string.IsNullOrEmpty(message))
            return ValidationMessages.InvalidValue;

        if (message.EndsWith("field is required.", StringComparison.Ordinal) || message.StartsWith("A non-empty request body is required", StringComparison.Ordinal))
            return ValidationMessages.Required;

        if (message.StartsWith("The JSON value", StringComparison.Ordinal) || message.StartsWith("The value", StringComparison.Ordinal) || message.Contains("could not be converted", StringComparison.Ordinal))
            return ValidationMessages.InvalidValue;

        return message;
    }
}
