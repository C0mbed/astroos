// src/AstroOS.UI/Services/Interfaces/ISiteProfileService.cs
namespace AstroOS.UI.Services.Interfaces;

/// <summary>
/// Provides site and observatory profile state. A valid site profile is one
/// of the four Session Ready conditions — without a configured location,
/// plate-solving, meridian flip planning, and altitude limits cannot function.
/// Status Bar Amendment v1.2.0 §1.
/// </summary>
public interface ISiteProfileService
{
    /// <summary>
    /// True when Settings → Site &amp; Observatory contains a fully-configured
    /// site profile (name, latitude, longitude, elevation). False if no profile
    /// exists or the profile is incomplete.
    /// </summary>
    bool HasValidSiteProfile { get; }

    /// <summary>
    /// Raised on the UI thread whenever HasValidSiteProfile changes (e.g., after
    /// the user saves or deletes a site profile in Settings).
    /// Consumers must not block this event handler.
    /// </summary>
    event EventHandler SiteProfileChanged;
}
