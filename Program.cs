using System.Text;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Dto;
using IpDeputyApi.Extensions;
using IpDeputyApi.Middleware;
using IpDeputyApi.OpenApi;
using IpDeputyApi.Options;
using IpDeputyApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions<AccessTokenOptions>().BindConfiguration(AccessTokenOptions.Section).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<RefreshTokenOptions>().BindConfiguration(RefreshTokenOptions.Section).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<BotOptions>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(DatabaseOptions.Section).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ScheduleOptions>().BindConfiguration(ScheduleOptions.Section);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context => context.ToValidationResult());

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<SecuritySchemeTransformer>();
    options.AddOperationTransformer<SecuritySchemeTransformer>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer()
    .AddScheme<BotAuthenticationSchemeOptions, BotAuthenticationHandler>(BotAuthenticationSchemeOptions.DefaultSchemeName, null);

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<AccessTokenOptions>>((options, accessOptions) =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = accessOptions.Value.Issuer,
            ValidAudience = accessOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessOptions.Value.Key)),
            NameClaimType = "name",
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddOptions<BotAuthenticationSchemeOptions>(BotAuthenticationSchemeOptions.DefaultSchemeName)
    .Configure<IOptions<BotOptions>>((options, botOptions) => options.BotToken = botOptions.Value.BotAuthorizeToken);

builder.Services.AddDbContextPool<IpDeputyDbContext>((services, options) =>
{
    options.UseNpgsql(services.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString);
});

builder.Services.AddRepositories();
builder.Services.AddAppServices();
builder.Services.AddSingleton<JwtService>();

var app = builder.Build();

app.UsePathBase("/api");

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (context.HttpContext.RequestAborted.IsCancellationRequested || response.HasStarted || response.ContentLength > 0 || !string.IsNullOrEmpty(response.ContentType))
        return;

    var message = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "Не авторизовано",
        StatusCodes.Status403Forbidden => "Недостатньо прав",
        StatusCodes.Status404NotFound => "Ресурс не знайдено",
        StatusCodes.Status405MethodNotAllowed => "Метод не підтримується",
        StatusCodes.Status429TooManyRequests => "Забагато запитів, спробуйте пізніше",
        _ => "Помилка запиту"
    };

    await response.WriteAsJsonAsync(new ErrorDto(response.StatusCode, message));
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(options =>
{
    var origins = app.Configuration.GetSection("Origins").Get<string[]>() ?? [];

    options.WithOrigins(origins);
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Services.GetRequiredService<IOptions<DatabaseOptions>>().Value.ApplyMigrationsOnStartup)
    await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();
