namespace Manimp.Shared.Models;

public class Tenant
{
    public Guid TenantId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string SecretRef { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

public class UserDirectory
{
    public int Id { get; set; }
    public string NormalizedEmail { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}

public class CompanyRegistrationRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
