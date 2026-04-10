// src/AstroOS.UI/Services/Interfaces/ISafetyService.cs
using AstroOS.UI.Models;

namespace AstroOS.UI.Services.Interfaces;

/// <summary>
/// Monitors safety conditions (limits, weather, mount state) and exposes
/// the aggregate SafetyStatus. The status feeds both the Zone A shield icon
/// and the Session Ready gate in StatusBarViewModel.
/// Status Bar Contract v1.1.0 §2.
/// </summary>
public interface ISafetyService
{
    /// <summary>
    /// Current aggregate safety status. AllClear or NoSource allows Session Ready.
    /// WarningActive or Unsafe blocks it.
    /// </summary>
    SafetyStatus SafetyStatus { get; }

    /// <summary>
    /// Raised on the UI thread whenever SafetyStatus changes.
    /// Consumers must not block this event handler.
    /// </summary>
    event EventHandler SafetyStatusChanged;
}
