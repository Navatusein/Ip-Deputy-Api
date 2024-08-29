using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Middleware;
using IpDeputyApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

// Add logger
builder.Host.UseSerilog();

builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Ip Deputy Bot API",
        Description = "API for telegram bot Ip Deputy Bot.",
        Contact = new OpenApiContact
        {
            Name = "Bohdan",
            Url = new Uri("https://github.com/Navatusein"),
            Email = "boghdan.kutsulima@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/Navatusein/Goose-Hub-Authentication-API/blob/main/LICENSE")
        },
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    var botApiKeySecurityScheme = new OpenApiSecurityScheme
    {
        Name = BotAuthenticationSchemeOptions.TokenHeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Put Bot API token on textbox below!",

        Reference = new OpenApiReference 
        { 
            Type = ReferenceType.SecurityScheme, 
            Id = BotAuthenticationSchemeOptions.DefaultSchemeName
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityDefinition(botApiKeySecurityScheme.Reference.Id, botApiKeySecurityScheme);

    options.OperationFilter<SecurityRequirementsOperationFilter>(true, jwtSecurityScheme.Reference.Id);
    options.OperationFilter<SecurityRequirementsOperationFilter>(true, botApiKeySecurityScheme.Reference.Id);
});

// Configure JWT Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
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
    

// Configure Database
builder.Services.AddDbContextPool<IpDeputyDbContext>(options =>
{
    options.UseLazyLoadingProxies();

    switch (builder.Configuration["Database:Provider"])
    {
        case "MySql":
            options.UseMySql(builder.Configuration["Database:ConnectionString"], new MySqlServerVersion(new Version(builder.Configuration["Database:Version"]!)));
            break;
        case "MariaDb":
            options.UseMySql(builder.Configuration["Database:ConnectionString"], new MariaDbServerVersion(new Version(builder.Configuration["Database:Version"]!)));
            break;
        case "PostgreSQL":
            options.UseNpgsql(builder.Configuration["Database:ConnectionString"]);
            break;
        default:
            throw new Exception("Invalid Database Provider");
    }
});

// Add JwtService
builder.Services.AddSingleton<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(option =>
    {
        option.RouteTemplate = "swagger/{documentName}/swagger.json";
        option.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> {
                new OpenApiServer {
                    Url = builder.Configuration.GetSection("BaseUrl").Get<string?>() ?? $"{httpReq.Scheme}://{httpReq.Host.Value}/"
                }
            };
        });
    });

    app.UseSwaggerUI(option =>
    {
        option.DocumentTitle = "Ip Deputy Api";
    });
}

// Add Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(options => {
    string[] origins = builder.Configuration.GetSection("Origins").Get<string[]>()!;

    options.WithOrigins(origins);
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
