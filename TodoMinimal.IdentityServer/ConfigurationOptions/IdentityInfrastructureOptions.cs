namespace TodoMinimal.IdentityServer.ConfigurationOptions;
public class IdentityInfrastructureOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new ConnectionStringsOptions();
    public AWSOptions AWS { get; set; } = new AWSOptions();
}