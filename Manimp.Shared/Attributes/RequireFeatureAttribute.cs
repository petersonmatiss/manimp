using System.ComponentModel.DataAnnotations;

namespace Manimp.Shared.Attributes;

/// <summary>
/// Attribute to gate API endpoints based on tenant feature access
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireFeatureAttribute : Attribute
{
    /// <summary>
    /// Gets the required feature key
    /// </summary>
    public string FeatureKey { get; }

    /// <summary>
    /// Gets or sets whether to return 404 instead of 403 when feature is disabled
    /// This hides the existence of the feature from unauthorized tenants
    /// </summary>
    public bool HideWhenDisabled { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the RequireFeatureAttribute
    /// </summary>
    /// <param name="featureKey">The feature key that must be enabled</param>
    public RequireFeatureAttribute(string featureKey)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
            throw new ArgumentException("Feature key cannot be null or empty", nameof(featureKey));
        
        FeatureKey = featureKey;
    }
}