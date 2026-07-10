using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UltraHotel.Infrastructure.Security;

namespace UltraHotel.WebApi.Extensions;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddHotelAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings settings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        byte[] key = Encoding.UTF8.GetBytes(settings.SecretKey);

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
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly",    p => p.RequireRole("ADMIN"))
            .AddPolicy("AgentOnly",    p => p.RequireRole("AGENT"))
            .AddPolicy("TravelerOnly", p => p.RequireRole("TRAVELER"));

        return services;
    }
}
