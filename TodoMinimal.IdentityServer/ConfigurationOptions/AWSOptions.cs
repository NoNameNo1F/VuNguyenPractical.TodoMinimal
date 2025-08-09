namespace TodoMinimal.IdentityServer.ConfigurationOptions
{
    public class AWSOptions
    {
        public string Region { get; set; } = string.Empty;
        public string UserPoolId { get; set; } = string.Empty;
        public string UserPoolClientId { get; set; } = string.Empty;
        public string UserPoolClientSecret { get; set; } = string.Empty;
        public string IdentityPoolId { get; set; } = string.Empty;
    }
}