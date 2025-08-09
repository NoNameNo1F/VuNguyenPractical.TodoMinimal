using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.SeedWork;
using TodoMinimal.Infrastructure.ConfigurationOptions;
using TodoMinimal.Infrastructure.Persistence;
using TodoMinimal.Infrastructure.Persistence.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class NoteInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddNoteInfrastructure(this IServiceCollection services, Action<NoteModuleOptions> configureOptions)
    {
        var settings = new NoteModuleOptions();
        configureOptions(settings);
        services.Configure(configureOptions);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(settings.ConnectionString.Default)
        );

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ICommand).Assembly);
        });
    
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
