using OpenIddict.Abstractions;

namespace TodoMinimal.IdentityServer.HostedServices;
public class Worker(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var app in OAuthConfig.Applications)
        {
            if (!string.IsNullOrEmpty(app.ClientId) && await manager.FindByClientIdAsync(app.ClientId, cancellationToken) is null)
            {
                await manager.CreateAsync(app, cancellationToken);
            }        }

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        foreach (var scp in OAuthConfig.Scopes)
        {
            if (!string.IsNullOrEmpty(scp.Name) && await scopeManager.FindByNameAsync(scp.Name, cancellationToken) is null)
            {
                await scopeManager.CreateAsync(scp, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}