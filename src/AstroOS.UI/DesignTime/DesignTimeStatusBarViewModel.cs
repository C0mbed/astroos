// src/AstroOS.UI/DesignTime/DesignTimeStatusBarViewModel.cs
// For use in XAML d:DataContext only — never instantiated at runtime.
using System.Windows.Input;
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;
using AstroOS.UI.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace AstroOS.UI.DesignTime;

// ── Null service — satisfies IConnectionService without real hardware ─────────

/// <summary>
/// No-op IConnectionService used only to construct ConnectionWidgetViewModels
/// inside design-time view models. Never raises events or performs I/O.
/// file-scoped so it cannot leak out of this compilation unit.
/// </summary>
file sealed class DesignNullConnectionService : IConnectionService
{
    public ConnectionState ConnectionState => ConnectionState.AllDisconnected;
    public int DeviceCount    => 0;
    public int ConnectedCount => 0;
    public int ErrorCount     => 0;

    public event EventHandler? ConnectionStateChanged { add { } remove { } }

    public Task ConnectAllAsync(CancellationToken ct = default)    => Task.CompletedTask;
    public Task DisconnectAllAsync(CancellationToken ct = default) => Task.CompletedTask;
}

// ── Shared base ───────────────────────────────────────────────────────────────

/// <summary>
/// Base class for all design-time Status Bar view models.
/// Mirrors the properties of StatusBarViewModel that StatusBar.xaml binds to,
/// so the XAML designer can render Zone A and Zone B without runtime services.
/// </summary>
public abstract class DesignTimeStatusBarViewModelBase
{
    protected DesignTimeStatusBarViewModelBase(
        ConnectionState state,
        int connectedCount,
        int deviceCount,
        int errorCount)
    {
        ConnectionWidget = new ConnectionWidgetViewModel(
            new DesignNullConnectionService(),
            () => { });

        ConnectionWidget.UpdateState(state, connectedCount, deviceCount, errorCount);
    }

    /// <summary>Zone B widget ViewModel — set as ConnectionWidget's DataContext by StatusBar.xaml.cs.</summary>
    public ConnectionWidgetViewModel ConnectionWidget { get; }

    /// <summary>Zone A safety panel drawer state. Always false in design-time.</summary>
    public bool IsSafetyPanelOpen => false;

    /// <summary>Zone A shield button AutomationProperties.Name.</summary>
    public string SafetyStatusAutomationLabel => "Safety Panel — All Systems Safe";

    /// <summary>Zone A shield button command. No-op in design-time.</summary>
    public ICommand OpenSafetyPanelCommand { get; } = new RelayCommand(() => { });
}

// ── Six concrete design-time VMs — one per Connection Widget visual state ─────
// Usage in StatusBar.xaml:
//   xmlns:dt="using:AstroOS.UI.DesignTime"
//   d:DataContext="{d:DesignInstance Type=dt:DesignTimeStatusBarAllDisconnected, IsDesignTimeCreatable=True}"

/// <summary>Design-time: Zone B shows "Disconnected" with "Connect All" chip.</summary>
public sealed class DesignTimeStatusBarAllDisconnected : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarAllDisconnected()
        : base(ConnectionState.AllDisconnected, connectedCount: 0, deviceCount: 5, errorCount: 0) { }
}

/// <summary>Design-time: Zone B shows "Connecting…" with spinner, no chip.</summary>
public sealed class DesignTimeStatusBarConnecting : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarConnecting()
        : base(ConnectionState.Connecting, connectedCount: 0, deviceCount: 5, errorCount: 0) { }
}

/// <summary>Design-time: Zone B shows "3/5 Connected" with "Connect All" chip.</summary>
public sealed class DesignTimeStatusBarPartiallyConnected : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarPartiallyConnected()
        : base(ConnectionState.PartiallyConnected, connectedCount: 3, deviceCount: 5, errorCount: 0) { }
}

/// <summary>Design-time: Zone B shows "5/5 Connected", no chip (all connected, not yet session-ready).</summary>
public sealed class DesignTimeStatusBarAllConnected : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarAllConnected()
        : base(ConnectionState.AllConnected, connectedCount: 5, deviceCount: 5, errorCount: 0) { }
}

/// <summary>Design-time: Zone B shows "Session Ready" with "Start →" chip.</summary>
public sealed class DesignTimeStatusBarSessionReady : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarSessionReady()
        : base(ConnectionState.SessionReady, connectedCount: 5, deviceCount: 5, errorCount: 0) { }
}

/// <summary>Design-time: Zone B shows "2 Errors" with "Retry" chip.</summary>
public sealed class DesignTimeStatusBarErrorPresent : DesignTimeStatusBarViewModelBase
{
    public DesignTimeStatusBarErrorPresent()
        : base(ConnectionState.ErrorPresent, connectedCount: 3, deviceCount: 5, errorCount: 2) { }
}
