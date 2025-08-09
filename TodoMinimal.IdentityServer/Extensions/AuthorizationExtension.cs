namespace TodoMinimal.IdentityServer.Extensions
{
    internal static class AuthorizationExtension
    {
        internal static IServiceCollection AddAuthorizationExtension(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}