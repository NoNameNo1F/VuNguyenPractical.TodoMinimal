using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace TodoMinimal.API.Extensions.SecurityExtensions
{
    internal static class AuthenticationExtension
    {
        internal static IServiceCollection AddSecurityExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer("https://localhost:8080/");
                    options.AddAudiences("resource_server_api1");

                    options.UseIntrospection()
                        .SetClientId("resource_server_api1")
                        .SetClientSecret("0633E55D-3851-4217-9BDA-F8A8AA6DB1FC");

                    options.UseAspNetCore();

                    options.UseSystemNetHttp();
                });
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("https://localhost:5001", "https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new HasScopeRequirement("api1", "https://localhost:8080/"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            return services;
        }
    }
}
