using TodoMinimal.Infrastructure.ConfigurationOptions;

namespace TodoMinimal.API.ConfigurationOptions;

public class AppSettings
{
    public string AllowedHosts { get; set; } = string.Empty;
    public Dictionary<string, object> Logging { get; set; } = new();
    public NoteModuleOptions NoteModule { get; set; } = new();
}