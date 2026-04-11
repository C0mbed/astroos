// src/AstroOS.UI/Services/Mock/MockSiteProfileService.cs
// DEBUG only — never registered in release builds.
using AstroOS.UI.Services.Interfaces;

namespace AstroOS.UI.Services.Mock;

/// <summary>
/// Mock ISiteProfileService for visual verification in DEBUG builds.
/// Always reports HasValidSiteProfile=true and never fires SiteProfileChanged,
/// satisfying the site-profile condition of the Session Ready gate unconditionally.
/// </summary>
internal sealed class MockSiteProfileService : ISiteProfileService
{
    public bool HasValidSiteProfile => true;

    // Event intentionally never raised.
    public event EventHandler? SiteProfileChanged { add { } remove { } }
}
