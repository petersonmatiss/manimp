using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Manimp.Shared.Attributes;
using Manimp.Shared.Interfaces;

namespace Manimp.Services.Middleware;

/// <summary>
/// Middleware to check feature gates for API endpoints
/// </summary>
public class FeatureGateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FeatureGateMiddleware> _logger;

    public FeatureGateMiddleware(RequestDelegate next, ILogger<FeatureGateMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IFeatureGate featureGate, ITenantService tenantService)
    {
        // Skip feature gating for non-API requests or if no endpoint is available
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // Check for RequireFeature attribute on the endpoint
        var requireFeatureAttribute = endpoint.Metadata.GetMetadata<RequireFeatureAttribute>();
        if (requireFeatureAttribute == null)
        {
            // No feature gate required, continue
            await _next(context);
            return;
        }

        // Get tenant ID from user claims or context
        var tenantId = await GetTenantIdFromContextAsync(context, tenantService);
        if (tenantId == null)
        {
            _logger.LogWarning("Feature gate check failed: No tenant ID found for request to {Path}", context.Request.Path);
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // Check if feature is enabled for the tenant
        try
        {
            var isFeatureEnabled = await featureGate.IsFeatureEnabledAsync(tenantId.Value, requireFeatureAttribute.FeatureKey);

            if (!isFeatureEnabled)
            {
                _logger.LogInformation("Feature gate denied access to {FeatureKey} for tenant {TenantId} on path {Path}",
                    requireFeatureAttribute.FeatureKey, tenantId, context.Request.Path);

                if (requireFeatureAttribute.HideWhenDisabled)
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Not Found");
                }
                else
                {
                    context.Response.StatusCode = 403; // Forbidden
                    await context.Response.WriteAsync("Feature not available for your subscription plan");
                }
                return;
            }

            _logger.LogDebug("Feature gate allowed access to {FeatureKey} for tenant {TenantId}",
                requireFeatureAttribute.FeatureKey, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature gate for {FeatureKey} and tenant {TenantId}",
                requireFeatureAttribute.FeatureKey, tenantId);

            // Fail closed - deny access on error
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
            return;
        }

        // Feature is enabled, continue to next middleware
        await _next(context);
    }

    /// <summary>
    /// Extracts the tenant ID from the HTTP context
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="tenantService">The tenant service</param>
    /// <returns>The tenant ID if found, null otherwise</returns>
    private async Task<Guid?> GetTenantIdFromContextAsync(HttpContext context, ITenantService tenantService)
    {
        // Try to get tenant ID from claims first (if user is authenticated)
        var tenantIdClaim = context.User.FindFirst("TenantId");
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantIdFromClaim))
        {
            return tenantIdFromClaim;
        }

        // If no tenant ID in claims, try to resolve from email (for authenticated users)
        var emailClaim = context.User.FindFirst("Email") ?? context.User.FindFirst("email");
        if (emailClaim != null)
        {
            var normalizedEmail = emailClaim.Value.ToUpperInvariant();
            var tenantIds = await tenantService.GetTenantIdsByEmailAsync(normalizedEmail);

            if (tenantIds.Count == 1)
            {
                return tenantIds[0];
            }
            else if (tenantIds.Count > 1)
            {
                _logger.LogWarning("Multiple tenant IDs found for email {Email}, cannot determine tenant context", emailClaim.Value);
            }
        }

        // Try to get tenant ID from custom header (for API calls with explicit tenant context)
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) &&
            Guid.TryParse(tenantHeader.FirstOrDefault(), out var tenantIdFromHeader))
        {
            return tenantIdFromHeader;
        }

        return null;
    }
}

/// <summary>
/// Extension methods for registering the FeatureGateMiddleware
/// </summary>
public static class FeatureGateMiddlewareExtensions
{
    /// <summary>
    /// Adds the feature gate middleware to the application pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseFeatureGate(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FeatureGateMiddleware>();
    }
}