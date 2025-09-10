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

// Tier 1 Core Inventory Schema

// Lookup Tables
public class MaterialType
{
    public int MaterialTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new List<ProfileInventory>();
}

public class ProfileType
{
    public int ProfileTypeId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "W-Beam", "Channel", "Angle"
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new List<ProfileInventory>();
}

public class SteelGrade
{
    public int SteelGradeId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "A992", "A36", "A572-50"
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new List<ProfileInventory>();
}

// Supporting entities for tagging
public class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new List<ProfileInventory>();
}

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileUsageLog> ProfileUsageLogs { get; set; } = new List<ProfileUsageLog>();
}

public class Document
{
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DocumentType { get; set; } // e.g., "Certificate", "Invoice"
    public string? FilePath { get; set; }
    public DateTime UploadedUtc { get; set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Navigation properties
    public ICollection<ProfileInventory> ProfileInventories { get; set; } = new List<ProfileInventory>();
}

// Core Inventory and Usage Tracking
public class ProfileInventory
{
    public int ProfileInventoryId { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty; // e.g., "W12x26", "L4x4x1/2"
    public decimal Length { get; set; } // in feet or meters
    public int PiecesOnHand { get; set; } // Current available pieces
    public int OriginalPieces { get; set; } // Original quantity received
    public decimal WeightPerPiece { get; set; } // lbs or kg per piece
    public decimal? UnitCost { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string? Location { get; set; } // Warehouse location
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Foreign Keys
    public int MaterialTypeId { get; set; }
    public int ProfileTypeId { get; set; }
    public int SteelGradeId { get; set; }
    public int? SupplierId { get; set; }
    public int? CertificateDocumentId { get; set; }
    
    // Navigation properties
    public MaterialType MaterialType { get; set; } = null!;
    public ProfileType ProfileType { get; set; } = null!;
    public SteelGrade SteelGrade { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public Document? CertificateDocument { get; set; }
    public ICollection<ProfileUsageLog> ProfileUsageLogs { get; set; } = new List<ProfileUsageLog>();
}

public class ProfileUsageLog
{
    public int ProfileUsageLogId { get; set; }
    public int PiecesUsed { get; set; }
    public decimal? LengthUsed { get; set; } // Optional: track partial lengths used
    public DateTime UsedDate { get; set; } = DateTime.UtcNow;
    public string? Purpose { get; set; } // What was it used for
    public string? UsedBy { get; set; } // Who used it (could be user ID later)
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = new byte[0];
    
    // Foreign Keys
    public int ProfileInventoryId { get; set; }
    public int? ProjectId { get; set; }
    
    // Navigation properties
    public ProfileInventory ProfileInventory { get; set; } = null!;
    public Project? Project { get; set; }
}
