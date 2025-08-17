using Amazon.Runtime.Credentials.Internal;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace TodoMinimal.IdentityServer.Extensions
{
    public static class OpenIdDictExtension
    {
        public static IServiceCollection AddOpenIdDictService(this IServiceCollection services, IConfiguration configuration)
        {
            var aws = new AWSOptions();
            configuration.GetSection("IdentityInfrastructure:AWS").Bind(aws);

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<IdentityContext>();
                })
                .AddServer(options =>
                {
                    options
                        .SetTokenEndpointUris("connect/token")
                        .SetAuthorizationEndpointUris("connect/authorize")
                        .SetEndSessionEndpointUris("connect/logout")
                        .SetIntrospectionEndpointUris("connect/introspect")
                        .SetUserInfoEndpointUris("connect/userinfo");

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.Roles,
                        "api1"
                    );

                    options.RequireProofKeyForCodeExchange();

                    options.AllowAuthorizationCodeFlow()
                        .AllowClientCredentialsFlow()
                        .AllowRefreshTokenFlow();

                    options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
                    options.UseReferenceRefreshTokens();
                    options.UseReferenceAccessTokens();

                    options.AddEncryptionKey(new SymmetricSecurityKey(
                       Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));
                    //.AddDevelopmentEncryptionCertificate()
                    options
                        .AddDevelopmentSigningCertificate();


                    options.UseAspNetCore()
                        // .DisableTransportSecurityRequirement()
                        .EnableStatusCodePagesIntegration()
                        .EnableAuthorizationEndpointPassthrough()
                        // .EnableTokenEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough();
                })
                .AddClient(options =>
                {
                    options.AllowAuthorizationCodeFlow();
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.UseAspNetCore()
                        .DisableTransportSecurityRequirement()
                        .EnableStatusCodePagesIntegration()
                        .EnableRedirectionEndpointPassthrough();

                    options.UseSystemNetHttp();

                    options.UseWebProviders()
                        .AddCognito(cfg =>
                        {
                            cfg.SetClientId(aws.UserPoolClientId)
                                .SetUserPoolId(aws.UserPoolId)
                                .SetRegion(aws.Region)
                                .SetRedirectUri("/signin-cognito")
                                .SetProviderDisplayName("Sign In With Cognito")
                                .AddScopes("openid email profile");
                        });

                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}