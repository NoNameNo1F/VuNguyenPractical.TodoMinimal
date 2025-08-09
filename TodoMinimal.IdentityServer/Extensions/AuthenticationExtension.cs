using Microsoft.AspNetCore.Identity;
using OpenIddict.Server.AspNetCore;

namespace TodoMinimal.IdentityServer.Extensions
{
    internal static class AuthenticationExtension
    {
        internal static IServiceCollection AddAuthenticationExtension(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
                // options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            // .AddCookie(IdentityConstants.ApplicationScheme, options =>
            // {

            //     options.LoginPath = "/Auth/Login";
            //     options.LogoutPath = "/Auth/Logout";
            //     options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //     options.SlidingExpiration = true;
            // });

            services.ConfigureApplicationCookie(options =>
            {
                // This tells the cookie handler where to redirect users when an action
                // requires them to be logged in.
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";

                // You can configure other cookie settings here as well.
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("https://localhost:5001", "https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            
            return services;
        }
    }
}