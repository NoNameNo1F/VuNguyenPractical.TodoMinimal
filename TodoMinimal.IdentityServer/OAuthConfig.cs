using OpenIddict.Abstractions;

namespace TodoMinimal.IdentityServer;

public class OAuthConfig
{
    public static IEnumerable<OpenIddictApplicationDescriptor> Applications => new List<OpenIddictApplicationDescriptor>
    {
        new OpenIddictApplicationDescriptor
        {
            ClientId = "amplify-client",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Amplify Web Client",
            Permissions =
            {
                // Endpoints
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Revocation,
                OpenIddictConstants.Permissions.Endpoints.EndSession,

                //GrantTypes
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                // Scopes - required role
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                // OpenIddictConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api1"
            },
            RedirectUris = {
                new Uri("https://localhost:3000/auth/callback"),
            },
            PostLogoutRedirectUris = 
            { 
                new Uri("https://localhost:3000/") 
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        },
        new OpenIddictApplicationDescriptor
        {
            ClientId = "resource_server_api1",
            ClientSecret = "0633E55D-3851-4217-9BDA-F8A8AA6DB1FC",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        }
    };

    public static IEnumerable<OpenIddictScopeDescriptor> Scopes => new List<OpenIddictScopeDescriptor>
    {
        new OpenIddictScopeDescriptor
        {
            Name = "api1",
            DisplayName = "Web API 1",
            Resources =
            {
                "resource_server_api1"
            }
        }
    };
}
