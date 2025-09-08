using Microsoft.AspNetCore.Mvc;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRegistrationService _companyRegistrationService;

    public CompanyController(ICompanyRegistrationService companyRegistrationService)
    {
        _companyRegistrationService = companyRegistrationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegistrationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName) ||
                string.IsNullOrWhiteSpace(request.AdminEmail) ||
                string.IsNullOrWhiteSpace(request.AdminPassword))
            {
                return BadRequest("All fields are required");
            }

            var tenantId = await _companyRegistrationService.RegisterCompanyAsync(request);

            return Ok(new { TenantId = tenantId, Message = "Company registered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Registration failed", Details = ex.Message });
        }
    }
}