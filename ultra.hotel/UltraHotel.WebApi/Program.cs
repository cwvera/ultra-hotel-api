using System.Text.Json.Serialization;
using UltraHotel.Application;
using UltraHotel.Infrastructure;
using UltraHotel.WebApi.Extensions;
using UltraHotel.WebApi.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHotelApiVersioning();
builder.Services.AddHotelAuthentication(builder.Configuration);
builder.Services.AddHotelCors(builder.Configuration);
builder.Services.AddHotelRateLimiting();
builder.Services.AddHotelSwagger();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

app.UseHotelSwagger();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsExtensions.PolicyName);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("general");
await app.RunAsync();
