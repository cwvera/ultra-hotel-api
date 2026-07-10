using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UltraHotel.Application.Features.Notifications.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Commons.Contracts;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Infrastructure.Elasticsearch;
using UltraHotel.Infrastructure.Messaging;
using UltraHotel.Infrastructure.Persistence;
using UltraHotel.Infrastructure.Persistence.Bookings;
using UltraHotel.Infrastructure.Persistence.Hotels;
using UltraHotel.Infrastructure.Persistence.Identity;
using UltraHotel.Infrastructure.Persistence.Rooms;
using UltraHotel.Infrastructure.Security;

namespace UltraHotel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Persistence
        services.AddDbContext<UltraHotelDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                sql => sql.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        // Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<ElasticsearchSettings>(configuration.GetSection("Elasticsearch"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));

        // Auth
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        // Repositories
        services.AddScoped<IUserRepository,    UserRepository>();
        services.AddScoped<IHotelRepository,   HotelRepository>();
        services.AddScoped<IRoomRepository,    RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Elasticsearch
        services.AddSingleton<IElasticsearchService, ElasticsearchService>();

        // RabbitMQ: publisher is singleton (reuses connection), consumer is hosted service
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddScoped<IEmailService, LogEmailService>();
        services.AddHostedService<BookingConfirmedConsumer>();

        return services;
    }
}
