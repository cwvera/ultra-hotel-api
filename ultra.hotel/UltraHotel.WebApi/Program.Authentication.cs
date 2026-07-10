using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UltraHotel.Infrastructure.Security;

partial class Program
{
    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings settings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        byte[]      key      = Encoding.UTF8.GetBytes(settings.SecretKey);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = settings.Issuer,
                    ValidAudience            = settings.Audience,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ClockSkew                = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly",    p => p.RequireRole("ADMIN"));
            options.AddPolicy("AgentOnly",    p => p.RequireRole("AGENT"));
            options.AddPolicy("TravelerOnly", p => p.RequireRole("TRAVELER"));
        });
    }
}
