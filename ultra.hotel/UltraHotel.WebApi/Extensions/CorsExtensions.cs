namespace UltraHotel.WebApi.Extensions;

internal static class CorsExtensions
{
    internal const string PolicyName = "UltraHotelCors";

    internal static IServiceCollection AddHotelCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
                // Sin orígenes configurados → sin CORS (secure by default)
            });
        });

        return services;
    }
}
