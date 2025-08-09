using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace TodoMinimal.API.Extensions.SecurityExtensions
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            var scopeClaim = context.User.FindFirst(c => c.Type == OpenIddictConstants.Claims.Scope && c.Issuer == requirement.Issuer);
            if (scopeClaim is null)
            {
                return Task.CompletedTask;
            }

            var scopes = scopeClaim.Value.Split(' ');

            if (scopes.Any(s => s == requirement.Scope))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}