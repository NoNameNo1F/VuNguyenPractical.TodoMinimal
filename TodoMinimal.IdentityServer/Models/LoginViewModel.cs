using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
namespace TodoMinimal.IdentityServer.Models;
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }

    // public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
}