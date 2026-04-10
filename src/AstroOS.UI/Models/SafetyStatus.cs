// src/AstroOS.UI/Models/SafetyStatus.cs
namespace AstroOS.UI.Models;

/// <summary>
/// The overall safety system status, as evaluated by ISafetyService.
/// Drives the Zone A shield icon appearance and is one of the four
/// Session Ready conditions (AllClear or NoSource required).
/// Status Bar Contract v1.1.0 §2 + Status Bar Amendment v1.2.0 §1.
/// </summary>
public enum SafetyStatus
{
    /// <summary>All safety checks pass. Session Ready gate: satisfied.</summary>
    AllClear,

    /// <summary>
    /// No safety source (e.g., weather station offline). The absence of a
    /// source is treated as non-unsafe — Session Ready gate: satisfied.
    /// Distinct from Unsafe so the user can choose to proceed without a weather feed.
    /// </summary>
    NoSource,

    /// <summary>
    /// A safety warning is active (e.g., rising humidity, approaching limit).
    /// Session Ready gate: NOT satisfied. User must acknowledge before imaging.
    /// </summary>
    WarningActive,

    /// <summary>
    /// Unsafe conditions detected (e.g., mount at hardware limit, weather danger).
    /// Session Ready gate: NOT satisfied. Sequence will not start.
    /// </summary>
    Unsafe,
}
