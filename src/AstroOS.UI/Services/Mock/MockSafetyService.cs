// src/AstroOS.UI/Services/Mock/MockSafetyService.cs
// DEBUG only — never registered in release builds.
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;

namespace AstroOS.UI.Services.Mock;

/// <summary>
/// Mock ISafetyService for visual verification in DEBUG builds.
/// Always reports AllClear and never fires SafetyStatusChanged,
/// satisfying the safety condition of the Session Ready gate unconditionally.
/// </summary>
internal sealed class MockSafetyService : ISafetyService
{
    public SafetyStatus SafetyStatus => SafetyStatus.AllClear;

    // Event intentionally never raised.
    public event EventHandler? SafetyStatusChanged { add { } remove { } }
}
