using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Manimp.Data.Contexts;
using Manimp.Shared.Models;

namespace Manimp.Services.Implementation;

/// <summary>
/// Service for managing CRM operations including customers, contacts, and projects
/// </summary>
public class CrmService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CrmService> _logger;

    public CrmService(AppDbContext context, ILogger<CrmService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Customer Management

    /// <summary>
    /// Gets all active customers with their primary contacts
    /// </summary>
    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.Contacts.Where(ct => ct.IsPrimary && ct.IsActive))
            .Where(c => c.IsActive)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a customer by ID with all related data
    /// </summary>
    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Contacts.Where(ct => ct.IsActive))
            .Include(c => c.CrmProjects.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created customer {CustomerName} with ID {CustomerId}", 
            customer.CompanyName, customer.CustomerId);
        
        return customer;
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated customer {CustomerName} with ID {CustomerId}", 
            customer.CompanyName, customer.CustomerId);
        
        return customer;
    }

    /// <summary>
    /// Soft deletes a customer by setting IsActive = false
    /// </summary>
    public async Task DeleteCustomerAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.IsActive = false;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Soft deleted customer {CustomerName} with ID {CustomerId}", 
                customer.CompanyName, customer.CustomerId);
        }
    }

    #endregion

    #region Contact Management

    /// <summary>
    /// Gets all contacts for a customer
    /// </summary>
    public async Task<List<Contact>> GetCustomerContactsAsync(int customerId)
    {
        return await _context.Contacts
            .Where(c => c.CustomerId == customerId && c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new contact for a customer
    /// </summary>
    public async Task<Contact> CreateContactAsync(Contact contact)
    {
        // If this is being set as primary, ensure only one primary contact per customer
        if (contact.IsPrimary)
        {
            await SetContactAsPrimaryAsync(contact.CustomerId, 0); // Clear existing primary
        }

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created contact {ContactName} for customer ID {CustomerId}", 
            contact.FullName, contact.CustomerId);
        
        return contact;
    }

    /// <summary>
    /// Updates an existing contact
    /// </summary>
    public async Task<Contact> UpdateContactAsync(Contact contact)
    {
        // If this is being set as primary, ensure only one primary contact per customer
        if (contact.IsPrimary)
        {
            await SetContactAsPrimaryAsync(contact.CustomerId, contact.ContactId);
        }

        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated contact {ContactName} with ID {ContactId}", 
            contact.FullName, contact.ContactId);
        
        return contact;
    }

    /// <summary>
    /// Sets a contact as the primary contact for a customer
    /// </summary>
    private async Task SetContactAsPrimaryAsync(int customerId, int primaryContactId)
    {
        var contacts = await _context.Contacts
            .Where(c => c.CustomerId == customerId && c.IsActive)
            .ToListAsync();

        foreach (var contact in contacts)
        {
            contact.IsPrimary = contact.ContactId == primaryContactId;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Soft deletes a contact by setting IsActive = false
    /// </summary>
    public async Task DeleteContactAsync(int contactId)
    {
        var contact = await _context.Contacts.FindAsync(contactId);
        if (contact != null)
        {
            contact.IsActive = false;
            
            // If this was the primary contact, make another contact primary if available
            if (contact.IsPrimary)
            {
                var anotherContact = await _context.Contacts
                    .Where(c => c.CustomerId == contact.CustomerId && c.IsActive && c.ContactId != contactId)
                    .FirstOrDefaultAsync();
                
                if (anotherContact != null)
                {
                    anotherContact.IsPrimary = true;
                }
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Soft deleted contact {ContactName} with ID {ContactId}", 
                contact.FullName, contact.ContactId);
        }
    }

    #endregion

    #region CRM Project Management

    /// <summary>
    /// Gets all active CRM projects
    /// </summary>
    public async Task<List<CrmProject>> GetCrmProjectsAsync()
    {
        return await _context.CrmProjects
            .Include(p => p.Customer)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedUtc)
            .ToListAsync();
    }

    /// <summary>
    /// Gets CRM projects for a specific customer
    /// </summary>
    public async Task<List<CrmProject>> GetCustomerCrmProjectsAsync(int customerId)
    {
        return await _context.CrmProjects
            .Include(p => p.Customer)
            .Include(p => p.AssemblyLists)
            .Where(p => p.CustomerId == customerId && p.IsActive)
            .OrderByDescending(p => p.CreatedUtc)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a CRM project by ID with related data
    /// </summary>
    public async Task<CrmProject?> GetCrmProjectByIdAsync(int crmProjectId)
    {
        return await _context.CrmProjects
            .Include(p => p.Customer)
            .Include(p => p.AssemblyLists)
                .ThenInclude(al => al.Assemblies)
            .Include(p => p.Deliveries)
            .FirstOrDefaultAsync(p => p.CrmProjectId == crmProjectId);
    }

    /// <summary>
    /// Creates a new CRM project
    /// </summary>
    public async Task<CrmProject> CreateCrmProjectAsync(CrmProject project)
    {
        _context.CrmProjects.Add(project);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created CRM project {ProjectName} with ID {ProjectId}", 
            project.Name, project.CrmProjectId);
        
        return project;
    }

    /// <summary>
    /// Updates an existing CRM project
    /// </summary>
    public async Task<CrmProject> UpdateCrmProjectAsync(CrmProject project)
    {
        _context.CrmProjects.Update(project);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated CRM project {ProjectName} with ID {ProjectId}", 
            project.Name, project.CrmProjectId);
        
        return project;
    }

    /// <summary>
    /// Updates the delivery information for a project
    /// </summary>
    public async Task UpdateProjectDeliveryAsync(int crmProjectId, DateTime? actualDeliveryDate, string? notes)
    {
        var project = await _context.CrmProjects.FindAsync(crmProjectId);
        if (project != null)
        {
            project.ActualDeliveryDate = actualDeliveryDate;
            if (!string.IsNullOrEmpty(notes))
            {
                project.Notes = string.IsNullOrEmpty(project.Notes) ? notes : $"{project.Notes}\n{notes}";
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated delivery information for project {ProjectId}", crmProjectId);
        }
    }

    /// <summary>
    /// Gets project status summary
    /// </summary>
    public async Task<Dictionary<string, int>> GetProjectStatusSummaryAsync()
    {
        return await _context.CrmProjects
            .Where(p => p.IsActive)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    #endregion

    #region Assembly and Manufacturing Progress

    /// <summary>
    /// Gets assembly lists for a project
    /// </summary>
    public async Task<List<AssemblyList>> GetProjectAssemblyListsAsync(int crmProjectId)
    {
        return await _context.AssemblyLists
            .Include(al => al.Assemblies)
            .Where(al => al.CrmProjectId == crmProjectId)
            .OrderByDescending(al => al.CreatedUtc)
            .ToListAsync();
    }

    /// <summary>
    /// Gets assemblies with their manufacturing progress
    /// </summary>
    public async Task<List<Assembly>> GetAssembliesWithProgressAsync(int assemblyListId)
    {
        return await _context.Assemblies
            .Include(a => a.AssemblyParts)
                .ThenInclude(ap => ap.Part)
            .Include(a => a.AssemblyCoatings)
                .ThenInclude(ac => ac.Coating)
            .Include(a => a.AssemblyOutsourcings)
                .ThenInclude(ao => ao.Supplier)
            .Where(a => a.AssemblyListId == assemblyListId)
            .OrderBy(a => a.AssemblyMark)
            .ToListAsync();
    }

    /// <summary>
    /// Updates manufacturing progress for an assembly
    /// </summary>
    public async Task UpdateAssemblyProgressAsync(int assemblyId, decimal progressPercentage, string? notes)
    {
        var assembly = await _context.Assemblies.FindAsync(assemblyId);
        if (assembly != null)
        {
            assembly.ProgressPercentage = Math.Max(0, Math.Min(100, progressPercentage)); // Clamp between 0-100
            
            if (!string.IsNullOrEmpty(notes))
            {
                assembly.ManufacturingNotes = string.IsNullOrEmpty(assembly.ManufacturingNotes) 
                    ? notes 
                    : $"{assembly.ManufacturingNotes}\n{DateTime.UtcNow:yyyy-MM-dd}: {notes}";
            }

            // Update manufacturing dates
            if (progressPercentage > 0 && assembly.ManufacturingStarted == null)
            {
                assembly.ManufacturingStarted = DateTime.UtcNow;
            }
            
            if (progressPercentage >= 100 && assembly.ManufacturingCompleted == null)
            {
                assembly.ManufacturingCompleted = DateTime.UtcNow;
            }
            else if (progressPercentage < 100)
            {
                assembly.ManufacturingCompleted = null; // Reset if progress goes backwards
            }

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated progress for assembly {AssemblyMark} to {Progress}%", 
                assembly.AssemblyMark, progressPercentage);
        }
    }

    /// <summary>
    /// Gets overall project progress based on assembly completion
    /// </summary>
    public async Task<decimal> GetProjectProgressAsync(int crmProjectId)
    {
        var assemblies = await _context.Assemblies
            .Where(a => a.AssemblyList.CrmProjectId == crmProjectId)
            .ToListAsync();

        if (!assemblies.Any())
            return 0;

        var totalProgress = assemblies.Sum(a => a.ProgressPercentage ?? 0);
        return totalProgress / assemblies.Count;
    }

    #endregion

    #region Delivery Management

    /// <summary>
    /// Creates a new delivery for a project
    /// </summary>
    public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created delivery {DeliveryNumber} for project {ProjectId}", 
            delivery.DeliveryNumber, delivery.CrmProjectId);
        
        return delivery;
    }

    /// <summary>
    /// Gets deliveries for a project
    /// </summary>
    public async Task<List<Delivery>> GetProjectDeliveriesAsync(int crmProjectId)
    {
        return await _context.Deliveries
            .Include(d => d.PackingLists)
                .ThenInclude(pl => pl.PackingListEntries)
            .Where(d => d.CrmProjectId == crmProjectId)
            .OrderByDescending(d => d.CreatedUtc)
            .ToListAsync();
    }

    /// <summary>
    /// Updates delivery status
    /// </summary>
    public async Task UpdateDeliveryStatusAsync(int deliveryId, string status, DateTime? actualDeliveryDate = null, string? notes = null)
    {
        var delivery = await _context.Deliveries.FindAsync(deliveryId);
        if (delivery != null)
        {
            delivery.Status = status;
            
            if (actualDeliveryDate.HasValue)
            {
                delivery.ActualDeliveryDate = actualDeliveryDate.Value;
            }
            
            if (!string.IsNullOrEmpty(notes))
            {
                delivery.Notes = string.IsNullOrEmpty(delivery.Notes) ? notes : $"{delivery.Notes}\n{notes}";
            }

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated delivery {DeliveryNumber} status to {Status}", 
                delivery.DeliveryNumber, status);
        }
    }

    #endregion
}