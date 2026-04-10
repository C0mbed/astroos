// src/AstroOS.UI/Services/Interfaces/IConnectionService.cs
using AstroOS.UI.Models;

namespace AstroOS.UI.Services.Interfaces;

/// <summary>
/// Manages hardware device connections. Reports aggregate connection state and
/// per-device counts. Fires ConnectionStateChanged whenever any device's state
/// changes so consumers (StatusBarViewModel) can update the Status Bar without polling.
/// Device Connect Contract v1.0.0 §2.
/// </summary>
public interface IConnectionService
{
    /// <summary>The raw aggregate connection state (never SessionReady — that is computed by StatusBarViewModel).</summary>
    ConnectionState ConnectionState { get; }

    /// <summary>Total number of configured devices (not including Not Configured entries).</summary>
    int DeviceCount { get; }

    /// <summary>Number of devices currently in the Connected state.</summary>
    int ConnectedCount { get; }

    /// <summary>Number of devices currently in the Error state.</summary>
    int ErrorCount { get; }

    /// <summary>
    /// Raised on the UI thread whenever ConnectionState, DeviceCount, ConnectedCount,
    /// or ErrorCount changes. Consumers must not block this event handler.
    /// </summary>
    event EventHandler ConnectionStateChanged;

    /// <summary>
    /// Initiates connection for all Disconnected and Offline devices simultaneously.
    /// Not Configured devices are skipped. Device Connect Contract v1.0.0 §1.9.
    /// </summary>
    Task ConnectAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Initiates disconnection for all currently-connected devices.
    /// Device Connect Contract v1.0.0 §1.9.
    /// </summary>
    Task DisconnectAllAsync(CancellationToken ct = default);
}
