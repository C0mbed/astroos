// src/AstroOS.UI/ViewModels/ConnectionWidgetViewModel.cs
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstroOS.UI.ViewModels;

/// <summary>
/// ViewModel for the ConnectionWidget UserControl (Status Bar Zone B).
/// Receives effective connection state from StatusBarViewModel and translates
/// it into the display values needed by the six ConnectionWidget visual states.
/// Device Connect Contract v1.0.0 §2.2 + Status Bar Amendment v1.2.0 §3.
/// </summary>
public sealed partial class ConnectionWidgetViewModel : ObservableObject
{
    private readonly IConnectionService _connectionService;
    private readonly Action _onStartSession;

    // ── Animation duration constants — Status Bar Amendment v1.2.0 §5 ──
    // These are referenced in code (GoToState useTransitions) and for documentation.
    // The actual XAML durations are specified in ConnectionWidget.xaml VisualTransitions.
    internal const int EnterSessionReadyMs = 300;
    internal const int LeaveSessionReadyMs = 150;
    internal const int DefaultStateTransitionMs = 200;

    /// <summary>
    /// Initialises the widget ViewModel.
    /// </summary>
    /// <param name="connectionService">
    /// Used to execute Connect All and Retry actions from the action chip.
    /// </param>
    /// <param name="onStartSession">
    /// Callback invoked when the user taps "Start →" in Session Ready state.
    /// Provided by StatusBarViewModel so navigation stays in the parent ViewModel.
    /// </param>
    public ConnectionWidgetViewModel(IConnectionService connectionService, Action onStartSession)
    {
        _connectionService = connectionService;
        _onStartSession = onStartSession;
    }

    // ── Observable state ──────────────────────────────────────────────────────

    /// <summary>
    /// Current effective connection state. Drives VisualStateManager in the view.
    /// Set by StatusBarViewModel.UpdateConnectionWidget() whenever the effective
    /// state (raw connection + Session Ready) changes.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LabelText))]
    [NotifyPropertyChangedFor(nameof(ShowChip))]
    [NotifyPropertyChangedFor(nameof(ChipLabel))]
    [NotifyPropertyChangedFor(nameof(IsSpinning))]
    [NotifyPropertyChangedFor(nameof(WidgetAutomationLabel))]
    [NotifyPropertyChangedFor(nameof(ChipAutomationLabel))]
    private ConnectionState _state = ConnectionState.AllDisconnected;

    /// <summary>
    /// Number of currently-connected devices. Used to build the label text for
    /// PartiallyConnected and AllConnected states.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LabelText))]
    private int _connectedCount;

    /// <summary>Total configured device count. Paired with ConnectedCount for fraction labels.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LabelText))]
    private int _deviceCount;

    /// <summary>Number of devices in the Error state. Drives the "[N] Error" label.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LabelText))]
    private int _errorCount;

    // ── Computed display values ───────────────────────────────────────────────

    /// <summary>
    /// Human-readable label for the widget's central text. Computed from State
    /// and device counts. Truncates with ellipsis at Zone B max-width (220px).
    /// Device Connect Contract v1.0.0 §2.2.
    /// </summary>
    public string LabelText => State switch
    {
        ConnectionState.AllDisconnected    => "Disconnected",
        ConnectionState.Connecting         => "Connecting\u2026",
        ConnectionState.PartiallyConnected => $"{ConnectedCount}/{DeviceCount} Connected",
        ConnectionState.AllConnected       => $"{ConnectedCount}/{DeviceCount} Connected",
        ConnectionState.SessionReady       => "Session Ready",
        ConnectionState.ErrorPresent       => ErrorCount == 1 ? "1 Error" : $"{ErrorCount} Errors",
        _                                  => string.Empty,
    };

    /// <summary>
    /// True when an action chip should be visible. Connecting and AllConnected
    /// states have no chip — there is no useful shortcut to offer.
    /// </summary>
    public bool ShowChip => State switch
    {
        ConnectionState.Connecting   => false,
        ConnectionState.AllConnected => false,
        _                            => true,
    };

    /// <summary>
    /// Text displayed on the action chip. "Start →" is the Session Ready CTA;
    /// "Connect All" covers disconnected/partial states; "Retry" covers error.
    /// Device Connect Contract v1.0.0 §2.2 + Amendment v1.2.0 §3.1.
    /// </summary>
    public string ChipLabel => State switch
    {
        ConnectionState.SessionReady => "Start \u2192",
        ConnectionState.ErrorPresent => "Retry",
        _                            => "Connect All",
    };

    /// <summary>
    /// True only in the Connecting state. Drives ProgressRing.IsActive in the view.
    /// All other states show a static status dot (filled Ellipse or hollow ring).
    /// </summary>
    public bool IsSpinning => State == ConnectionState.Connecting;

    /// <summary>
    /// Accessibility label for the action chip button. Announced by screen readers
    /// so AT users understand the action without reading the visual label.
    /// Status Bar Amendment v1.2.0 §6.
    /// </summary>
    public string ChipAutomationLabel => State switch
    {
        ConnectionState.SessionReady => "Start imaging session",
        ConnectionState.ErrorPresent => "Retry device connection",
        _                            => "Connect all devices",
    };

    /// <summary>
    /// Accessibility label for the widget button. Announced by screen readers.
    /// Status Bar Amendment v1.2.0 §6.
    /// </summary>
    public string WidgetAutomationLabel => State switch
    {
        ConnectionState.SessionReady => "Device connections, session ready",
        _                            => "Device connections",
    };

    // ── State update ──────────────────────────────────────────────────────────

    /// <summary>
    /// Called by StatusBarViewModel whenever the effective connection state or
    /// device counts change. Batch-updates all observable properties so the
    /// view only re-renders once per service event.
    /// </summary>
    public void UpdateState(
        ConnectionState effectiveState,
        int connectedCount,
        int deviceCount,
        int errorCount)
    {
        // Set backing fields directly to avoid partial notification on each property.
        // OnPropertyChanged is raised once per property in the setters below.
        ConnectedCount = connectedCount;
        DeviceCount    = deviceCount;
        ErrorCount     = errorCount;
        State          = effectiveState; // State last — triggers all [NotifyPropertyChangedFor]
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Executed when the user taps the action chip. Behaviour depends on state:
    /// Session Ready → navigate to Sequencer; otherwise → attempt Connect All / Retry.
    /// Device Connect Contract v1.0.0 §2.2 + Amendment v1.2.0 §2.4.
    /// </summary>
    [RelayCommand]
    private async Task ChipActionAsync()
    {
        if (State == ConnectionState.SessionReady)
        {
            _onStartSession();
        }
        else
        {
            // Connect All covers AllDisconnected, PartiallyConnected, and ErrorPresent (Retry).
            await _connectionService.ConnectAllAsync();
        }
    }
}
