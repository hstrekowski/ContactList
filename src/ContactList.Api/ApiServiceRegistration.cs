using System.Text;
using ContactList.Api.Common;
using ContactList.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ContactList.Api;
public static class ApiServiceRegistration
{
    public const string AngularCorsPolicy = "AllowAngularDevClient";

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSwaggerWithJwt();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();
        services.AddAngularCors();

        return services;
    }
    private static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ContactList API",
                Version = "v1",
                Description = "REST API for the ContactList recruitment task.",
            });

            var jwtScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Paste a JWT obtained from POST /api/auth/login. The 'Bearer ' prefix is added automatically.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            };

            options.AddSecurityDefinition("Bearer", jwtScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [jwtScheme] = Array.Empty<string>(),
            });
        });

        return services;
    }
    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }
    private static IServiceCollection AddAngularCors(this IServiceCollection services)
    {
        services.AddCors(o => o.AddPolicy(AngularCorsPolicy, p => p
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()));

        return services;
    }
}
