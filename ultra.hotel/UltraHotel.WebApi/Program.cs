using System.Text.Json.Serialization;
using UltraHotel.Application;
using UltraHotel.Infrastructure;
using UltraHotel.WebApi.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

ConfigureApiVersioning(builder.Services);
ConfigureAuthentication(builder.Services, builder.Configuration);
ConfigureCors(builder.Services, builder.Configuration);
ConfigureRateLimiting(builder.Services);
ConfigureSwagger(builder.Services);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

UseSwagger(app);

app.UseMiddleware<ExceptionHandlingMiddleware>();
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("general");
app.Run();
