using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Models;

/// <summary>
/// Represents a tenant in the multi-tenant system
/// </summary>
public class Tenant
{
    /// <summary>
    /// Gets or sets the unique identifier for the tenant
    /// </summary>
    public Guid TenantId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the tenant name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database name for this tenant
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret reference for database connection
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SecretRef { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the tenant is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the UTC creation date
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Maps user emails to their tenant
/// </summary>
public class UserDirectory
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the normalized email address
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string NormalizedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant identifier
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the associated tenant
    /// </summary>
    public Tenant? Tenant { get; set; }
}