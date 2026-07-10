using Microsoft.OpenApi;

partial class Program
{
    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "UltraHotel API",
                Version     = "v1",
                Description = "API de gestión y reserva de hoteles — UltraGroup Tech.",
                Contact     = new OpenApiContact { Name = "UltraGroup", Email = "dev@ultragroup.com" }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.ApiKey,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Introduce el JWT obtenido en /auth/login. Formato: Bearer {token}"
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", doc), [] }
            });

            string webApiXml = Path.Combine(AppContext.BaseDirectory, "UltraHotel.WebApi.xml");
            string appXml    = Path.Combine(AppContext.BaseDirectory, "UltraHotel.Application.xml");
            if (File.Exists(webApiXml)) options.IncludeXmlComments(webApiXml);
            if (File.Exists(appXml))    options.IncludeXmlComments(appXml);
        });
    }

    private static void UseSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "UltraHotel API v1");
            options.DocumentTitle = "UltraHotel API — Docs";
            options.DefaultModelsExpandDepth(-1);
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            options.EnableDeepLinking();
            options.EnableFilter();
            options.EnableTryItOutByDefault();
        });
    }
}
