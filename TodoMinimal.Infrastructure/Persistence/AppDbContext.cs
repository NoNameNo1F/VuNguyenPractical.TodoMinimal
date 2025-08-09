using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TodoMinimal.Domain.Notes;
using TodoMinimal.Infrastructure.ConfigurationOptions;

namespace TodoMinimal.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Ensure the database is created when the context is initialized
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Adding configuration
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public class TelegramBotDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../TodoMinimal.API");

                // Load configuration from appsettings.json
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                    .Build();

                var noteModuleOptions = new NoteModuleOptions();
                configuration.GetSection("NoteModule").Bind(noteModuleOptions);
                var connectionString = noteModuleOptions.ConnectionString.Default;

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new AppDbContext(optionsBuilder.Options);
            }
        }
    }
}

