using Microsoft.AspNetCore.Mvc;
using Manimp.Shared.Attributes;
using Manimp.Shared.Models;
using Manimp.Shared.Interfaces;

namespace Manimp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IFeatureGate _featureGate;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IFeatureGate featureGate, ILogger<InventoryController> logger)
    {
        _featureGate = featureGate;
        _logger = logger;
    }

    /// <summary>
    /// Get basic inventory information - available to all tiers
    /// </summary>
    [HttpGet("profiles")]
    [RequireFeature(FeatureKeys.CoreInventory)]
    public IActionResult GetProfiles()
    {
        return Ok(new { message = "Basic inventory profiles - available to all plans" });
    }

    /// <summary>
    /// Get purchase orders - only available to Professional and Enterprise tiers
    /// </summary>
    [HttpGet("purchase-orders")]
    [RequireFeature(FeatureKeys.PurchaseOrders)]
    public IActionResult GetPurchaseOrders()
    {
        return Ok(new { message = "Purchase orders - available to Professional and Enterprise plans" });
    }

    /// <summary>
    /// Get remnant tracking - only available to Professional and Enterprise tiers
    /// </summary>
    [HttpGet("remnants")]
    [RequireFeature(FeatureKeys.RemnantTracking)]
    public IActionResult GetRemnants()
    {
        return Ok(new { message = "Remnant tracking - available to Professional and Enterprise plans" });
    }

    /// <summary>
    /// Get price requests - only available to Enterprise tier
    /// </summary>
    [HttpGet("price-requests")]
    [RequireFeature(FeatureKeys.PriceRequests)]
    public IActionResult GetPriceRequests()
    {
        return Ok(new { message = "Price requests - available to Enterprise plan only" });
    }

    /// <summary>
    /// Get sourcing management - only available to Enterprise tier
    /// </summary>
    [HttpGet("sourcing")]
    [RequireFeature(FeatureKeys.SourcingManagement, HideWhenDisabled = true)]
    public IActionResult GetSourcing()
    {
        return Ok(new { message = "Sourcing management - available to Enterprise plan only" });
    }

    /// <summary>
    /// Get tenant features for debugging
    /// </summary>
    [HttpGet("features")]
    public IActionResult GetTenantFeatures()
    {
        // This would normally get tenant ID from authentication context
        // For demo purposes, we'll return an empty response
        return Ok(new { message = "Feature check endpoint - requires tenant context" });
    }
}