using Microsoft.AspNetCore.Identity;

namespace Manimp.Auth.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
