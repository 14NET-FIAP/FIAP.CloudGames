using Microsoft.OpenApi.Models;

namespace FIAP.CloudGames.API.Config;

public static class ControllerConfiguration
{
    public static IServiceCollection AddSwaggerConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });

            // Define o esquema de segurança (Bearer Token)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT neste formato: Bearer {seu_token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Aplica a exigência do token globalmente nas requisições do Swagger
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
