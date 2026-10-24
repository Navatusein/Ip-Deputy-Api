using IpDeputyApi.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IpDeputyApi.OpenApi;

public class SecuritySchemeTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT access token"
        };

        document.Components.SecuritySchemes[BotAuthenticationSchemeOptions.DefaultSchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = BotAuthenticationSchemeOptions.TokenHeaderName,
            In = ParameterLocation.Header,
            Description = "Bot API token"
        };

        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        if (metadata.OfType<IAllowAnonymous>().Any())
            return Task.CompletedTask;

        var authorizeData = metadata.OfType<IAuthorizeData>().ToList();

        if (authorizeData.Count == 0)
            return Task.CompletedTask;

        var isBot = authorizeData.Any(x => x.AuthenticationSchemes?.Contains(BotAuthenticationSchemeOptions.DefaultSchemeName) == true);
        var schemeName = isBot ? BotAuthenticationSchemeOptions.DefaultSchemeName : JwtBearerDefaults.AuthenticationScheme;

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeName, context.Document)] = new List<string>()
        });

        return Task.CompletedTask;
    }
}
