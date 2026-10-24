using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Middleware;
using IpDeputyApi.OpenApi;
using IpDeputyApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<SecuritySchemeTransformer>();
    options.AddOperationTransformer<SecuritySchemeTransformer>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AuthorizeJWT:Issuer"],
            ValidAudience = builder.Configuration["AuthorizeJWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthorizeJWT:Key"]!))
        };
    })
    .AddScheme<BotAuthenticationSchemeOptions, BotAuthenticationHandler>(
        BotAuthenticationSchemeOptions.DefaultSchemeName,
        options =>
        {
            options.BotToken = builder.Configuration["BotAuthorizeToken"]!;
        }
    );

builder.Services.AddDbContextPool<IpDeputyDbContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseNpgsql(builder.Configuration["Database:ConnectionString"]);
});

builder.Services.AddSingleton<JwtService>();

builder.Services.AddAutoMapper(typeof(AppMappingService));

var app = builder.Build();

app.UsePathBase("/api");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(options =>
{
    var origins = builder.Configuration.GetSection("Origins").Get<string[]>()!;

    options.WithOrigins(origins);
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
