using Microsoft.OpenApi;
using System.Reflection;

namespace StudentsRegistrationSystem.API.Extensions;

public static class SwaggerCollection
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Students Registration API",
                Description = "ASP.NET Core Web API for managing students, courses, and enrollments",
                Version = "v1"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        return services;
    }
}