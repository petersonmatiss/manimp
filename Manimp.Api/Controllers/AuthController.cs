using Microsoft.AspNetCore.Mvc;
using Manimp.Shared.Interfaces;
using Manimp.Shared.Models;

namespace Manimp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IAuthService _authService;

    public AuthController(ITenantService tenantService, IAuthService authService)
    {
        _tenantService = tenantService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            // Get tenant IDs for this email
            var normalizedEmail = request.Email.ToUpperInvariant();
            var tenantIds = await _tenantService.GetTenantIdsByEmailAsync(normalizedEmail);

            if (!tenantIds.Any())
            {
                return Unauthorized("Invalid email or password");
            }

            // Try to authenticate against each tenant
            foreach (var tenantId in tenantIds)
            {
                var isValid = await _authService.ValidateUserAsync(request.Email, request.Password, tenantId);
                if (isValid)
                {
                    // Set up authentication session - simplified for Phase 1
                    return Ok(new { TenantId = tenantId, Message = "Login successful" });
                }
            }

            return Unauthorized("Invalid email or password");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Login failed", Details = ex.Message });
        }
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, [FromQuery] Guid tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            var userId = await _authService.CreateUserAsync(request, tenantId);
            
            // Add user to directory
            await _tenantService.AddUserToDirectoryAsync(request.Email.ToUpperInvariant(), tenantId);

            return Ok(new { UserId = userId, Message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "User creation failed", Details = ex.Message });
        }
    }
}