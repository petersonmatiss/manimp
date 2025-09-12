// This file serves as a global access point for all domain models.
// The actual model classes are now organized in separate files by domain area:
//
// - Models/Tenant.cs - Multi-tenant system entities (Tenant, UserDirectory)
// - Models/Authentication.cs - Authentication and user management DTOs
// - Models/LookupTables.cs - Reference data entities (MaterialType, ProfileType, SteelGrade)  
// - Models/SupportingEntities.cs - Supporting business entities (Supplier, Project, Document)
// - Models/CoreInventory.cs - Core inventory management entities (ProfileInventory, ProfileUsageLog)
// - Models/Procurement.cs - Tier 2 procurement and remnants entities (PurchaseOrder, etc.)
//
// This modular organization improves maintainability, follows Single Responsibility Principle,
// and makes the codebase easier to navigate and understand.

// Re-export all model namespaces for backward compatibility
global using Manimp.Shared.Models;
