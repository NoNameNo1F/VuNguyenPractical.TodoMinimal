using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Microsoft.AspNetCore.Identity;
using TodoMinimal.IdentityServer.Models;
using OpenIddict.Client.AspNetCore;

namespace TodoMinimal.IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;

        public AuthController( 
            UserManager<IdentityUser<Guid>> userManager,
            SignInManager<IdentityUser<Guid>> signInManager,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Challenge(string provider, string? returnUrl = null)
        // {
        //     return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, provider);
        // }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,

                // ExternalLogins = (await HttpContext.Get).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password, 
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return LocalRedirect(model.ReturnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser<Guid> 
            { 
                UserName = model.Email, 
                Email = model.Email 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                return LocalRedirect(model.ReturnUrl ?? "/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet("~/connect/logout"), HttpPost("~/connect/logout")]
        public async Task<IActionResult> LogoutEndpoint()
        {
            await _signInManager.SignOutAsync();

            return SignOut(authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet("~/connect/authorize")]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() 
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if(!User.Identity!.IsAuthenticated)
            {
                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.Query.Select(parameter => new KeyValuePair<string, string?>(parameter.Key, parameter.Value)))
                    });
            }

            if (string.IsNullOrEmpty(request.ClientId))
            {
                throw new InvalidOperationException("The client_id is missing from the request.");
            }
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The specified client application cannot be found.");

            var user = await _userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("The user cannot be found.");

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: OpenIddictConstants.Claims.Name, 
                roleType: OpenIddictConstants.Claims.Role
            );

            var userId = user.Id.ToString();
            identity.SetClaim(OpenIddictConstants.Claims.Subject,
                userId);

            identity.SetClaim(
                OpenIddictConstants.Claims.Email,
                user.Email
            );
            identity.SetClaim(
                OpenIddictConstants.Claims.Name,
                user.UserName
            );

            identity.SetScopes(request.GetScopes());
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            
            // identity.SetDestinations(claim => claim.Type switch
            // {
            //     OpenIddictConstants.Claims.Name when claim.Subject!.HasScope(OpenIddictConstants.Scopes.Profile)
            //         => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            //     OpenIddictConstants.Claims.Role when claim.Subject!.HasScope(OpenIddictConstants.Scopes.Roles)
            //         => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            //     _ => [OpenIddictConstants.Destinations.AccessToken]
            // });

             identity.SetDestinations(claim => claim.Type switch
            {
                OpenIddictConstants.Claims.Subject when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Email when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Name when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Role when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Roles)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                _ => [OpenIddictConstants.Destinations.AccessToken]
            });

            return SignIn(
                new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
        }


        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if(!request.IsAuthorizationCodeGrantType() && 
            !request.IsRefreshTokenGrantType())    
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var userId = result.Principal!.GetClaim(OpenIddictConstants.Claims.Subject);
            if (userId is null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cannot find user from the token."
                    }));
            }

            var identity = new ClaimsIdentity(
                result.Principal!.Claims,
                TokenValidationParameters.DefaultAuthenticationType, OpenIddictConstants.Claims.Name,
                OpenIddictConstants.Claims.Role
            );

            var userIdClaim = result.Principal.GetClaim(OpenIddictConstants.Claims.Subject);
            if (userIdClaim is null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cannot find user id from the token."
                    }));
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cannot find user from the token."
                    }));
            }

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(OpenIddictConstants.Claims.Role, role);
            }

            identity.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
            identity.SetClaim(OpenIddictConstants.Claims.Email, user.Email);
            identity.SetClaim(OpenIddictConstants.Claims.Name, user.UserName);

            identity.SetScopes(request.GetScopes());
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

            identity.SetDestinations(claim => claim.Type switch
            {
                OpenIddictConstants.Claims.Subject when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Email when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Name when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                OpenIddictConstants.Claims.Role when claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Roles)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                _ => [OpenIddictConstants.Destinations.AccessToken]
            });

            // identity.SetDestinations(claim =>
            // {
            //     // The "sub" claim is essential for both token types.
            //     if (claim.Type == OpenIddictConstants.Claims.Subject)
            //     {
            //         return [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken];
            //     }
                
            //     // The "name" and "email" claims should only be added to the identity token
            //     // if the "profile" scope was granted.
            //     if (claim.Type is OpenIddictConstants.Claims.Name or OpenIddictConstants.Claims.Email)
            //     {
            //         if (claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile))
            //         {
            //             return [OpenIddictConstants.Destinations.IdentityToken];
            //         }
            //         // If profile scope is not granted, these claims are not included anywhere.
            //         return []; 
            //     }

            //     // The "role" claim should be added to both the access token (for API authorization)
            //     // and the identity token if the "roles" scope was granted.
            //     if (claim.Type == OpenIddictConstants.Claims.Role)
            //     {
            //         if (claim.Subject != null && claim.Subject.HasScope(OpenIddictConstants.Scopes.Roles))
            //         {
            //             return [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken];
            //         }
            //         return [];
            //     }
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

        [HttpGet("~/signin-cognito")]
        [HttpPost("~/signin-cognito")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogInCognitoCallback(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login", new { });
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email) ??
                result.Principal.FindFirstValue(OpenIddictConstants.Claims.Email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", new { });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new IdentityUser<Guid> 
                { 
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true 
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    return RedirectToAction("Login", new { });
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var redirectUri = result.Properties.RedirectUri ?? Url.Action("Login", "Auth");
            return Redirect(redirectUri ?? "/");
        }
    }
}