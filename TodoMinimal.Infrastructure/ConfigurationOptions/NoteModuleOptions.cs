namespace TodoMinimal.Infrastructure.ConfigurationOptions
{
    public class NoteModuleOptions
    {
        public ConnectionStringOptions ConnectionString { get; set; } = new();
    }
}