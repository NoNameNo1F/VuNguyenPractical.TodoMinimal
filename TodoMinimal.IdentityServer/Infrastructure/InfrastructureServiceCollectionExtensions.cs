using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<IdentityInfrastructureOptions> configureOptions)
        {
            var settings = new IdentityInfrastructureOptions();
            configureOptions(settings);
            services.Configure(configureOptions);

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(settings.ConnectionStrings.Identity);
                options.UseOpenIddict();
            });

            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}