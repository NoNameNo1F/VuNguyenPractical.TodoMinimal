using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;

namespace TodoMinimal.API.Extensions
{
    internal static class SwaggerExtension
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddFastEndpoints()
                .SwaggerDocument(options =>
                {
                    options.DocumentSettings = s => 
                    {
                        s.Title = "Note Demo API";
                        s.Version = "v1.0";
                        s.Description = "Note Clean .NET Backend.";
                        s.DocumentName = "v1";
                        s.AddAuth("ApiKey", new()
                        {
                            //https://gist.github.com/dj-nitehawk/4efe5ef70f813aec2c55fff3bbb833c0
                            Name = "api_key",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Type = OpenApiSecuritySchemeType.ApiKey,
                        });
                        // s.AddAuth("Bearer", new()
                        // {
                        //     Type = OpenApiSecuritySchemeType.Http,
                        //     Scheme = JwtBearerDefaults.AuthenticationScheme,
                        //     BearerFormat = "JWT",
                        // });
                    };
                });

            return services;
        }

        internal static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Errors.UseProblemDetails();
            });
            app.UseSwaggerGen();

            return app;
        }
    }
}
