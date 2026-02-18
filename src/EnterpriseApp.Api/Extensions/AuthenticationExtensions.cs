using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseApp.Api.Extensions;

/// <summary>
/// Extension methods for configuring authentication.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication configuration.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Secret"] ??
            throw new InvalidOperationException("JWT Secret is not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Adds authorization policies.
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy("RequireAdministrator", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("RequireManager", policy =>
                policy.RequireRole("Administrator", "Manager"));

            options.AddPolicy("RequireVertrieb", policy =>
                policy.RequireRole("Administrator", "Manager", "Vertrieb"));

            options.AddPolicy("RequireLager", policy =>
                policy.RequireRole("Administrator", "Manager", "Lager"));

            // Permission-based policies
            options.AddPolicy("CanManageCustomers", policy =>
                policy.RequireClaim("permission", "customers:write"));

            options.AddPolicy("CanManageProducts", policy =>
                policy.RequireClaim("permission", "products:write"));

            options.AddPolicy("CanManageOrders", policy =>
                policy.RequireClaim("permission", "orders:write"));

            options.AddPolicy("CanManageInventory", policy =>
                policy.RequireClaim("permission", "inventory:write"));

            options.AddPolicy("CanViewReports", policy =>
                policy.RequireClaim("permission", "reports:read"));

            options.AddPolicy("CanManageUsers", policy =>
                policy.RequireRole("Administrator"));
        });

        return services;
    }

    /// <summary>
    /// Adds rate limiting configuration.
    /// </summary>
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global rate limit
            options.AddFixedWindowLimiter("fixed", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Strict rate limit for authentication endpoints
            options.AddFixedWindowLimiter("auth", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });

            // API rate limit per user
            options.AddSlidingWindowLimiter("api", limiterOptions =>
            {
                limiterOptions.PermitLimit = 1000;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.SegmentsPerWindow = 6;
                limiterOptions.QueueLimit = 50;
            });
        });

        return services;
    }
}
