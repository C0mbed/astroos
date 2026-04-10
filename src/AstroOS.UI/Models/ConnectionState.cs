// src/AstroOS.UI/Models/ConnectionState.cs
namespace AstroOS.UI.Models;

/// <summary>
/// Represents the effective connection state shown in the Status Bar Connection Widget
/// (Zone B). This is the display state — it combines raw device connection data from
/// IConnectionService with the computed Session Ready signal from StatusBarViewModel.
/// Values map 1:1 to the six visual states in ConnectionWidget.xaml.
/// Device Connect Contract v1.0.0 §2.2 + Status Bar Amendment v1.2.0 §3.
/// </summary>
public enum ConnectionState
{
    /// <summary>No devices are connected. Shows "Disconnected" label and "Connect All" chip.</summary>
    AllDisconnected,

    /// <summary>One or more devices are actively attempting to connect. Shows spinner, no chip.</summary>
    Connecting,

    /// <summary>Some but not all configured devices are connected. Shows warning dot and "Connect All" chip.</summary>
    PartiallyConnected,

    /// <summary>All devices connected but Session Ready conditions are not all satisfied.</summary>
    AllConnected,

    /// <summary>
    /// All four Session Ready conditions are simultaneously met: all devices connected,
    /// no safety warnings, valid site profile, sequencer idle or stopped.
    /// Shows green "Session Ready" label and "Start →" chip.
    /// Status Bar Amendment v1.2.0 §1.
    /// </summary>
    SessionReady,

    /// <summary>One or more devices are in an error state. Shows error dot and "Retry" chip.</summary>
    ErrorPresent,
}
