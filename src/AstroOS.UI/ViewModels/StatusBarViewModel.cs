// src/AstroOS.UI/ViewModels/StatusBarViewModel.cs
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstroOS.UI.ViewModels;

/// <summary>
/// ViewModel for the StatusBar UserControl. Owns Zone A (Safety Panel trigger)
/// and Zone B (Connection Widget). Subscribes to four services and computes
/// IsSessionReady from their combined state.
/// Status Bar Contract v1.1.0 + Amendment v1.2.0.
/// </summary>
public sealed partial class StatusBarViewModel : ObservableObject, IDisposable
{
    private readonly IConnectionService  _connectionService;
    private readonly ISafetyService      _safetyService;
    private readonly ISequencerService   _sequencerService;
    private readonly ISiteProfileService _siteProfileService;
    private bool _disposed;

    // ── Constructor ───────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises the Status Bar ViewModel. Subscribes to all four service change
    /// events and creates the ConnectionWidgetViewModel with an initial state.
    /// </summary>
    public StatusBarViewModel(
        IConnectionService  connectionService,
        ISafetyService      safetyService,
        ISequencerService   sequencerService,
        ISiteProfileService siteProfileService)
    {
        _connectionService  = connectionService;
        _safetyService      = safetyService;
        _sequencerService   = sequencerService;
        _siteProfileService = siteProfileService;

        // Initialise observable fields from current service state.
        _deviceCount            = _connectionService.DeviceCount;
        _connectedCount         = _connectionService.ConnectedCount;
        _errorCount             = _connectionService.ErrorCount;
        _currentSafetyStatus    = _safetyService.SafetyStatus;
        _hasValidSiteProfile    = _siteProfileService.HasValidSiteProfile;
        _currentSequencerState  = _sequencerService.SequencerState;

        // Create the Zone B ViewModel. Navigation callback delegates to our command.
        ConnectionWidget = new ConnectionWidgetViewModel(
            _connectionService,
            onStartSession: () => NavigateToSequencerCommand.Execute(null));

        // Compute effective connection state and push to widget.
        UpdateEffectiveConnectionState();

        // Subscribe to service events.
        _connectionService.ConnectionStateChanged  += OnConnectionServiceChanged;
        _safetyService.SafetyStatusChanged         += OnSafetyServiceChanged;
        _sequencerService.SequencerStateChanged    += OnSequencerServiceChanged;
        _siteProfileService.SiteProfileChanged     += OnSiteProfileServiceChanged;
    }

    // ── Zone B — Connection Widget ────────────────────────────────────────────

    /// <summary>
    /// The ViewModel for the ConnectionWidget resident in Zone B.
    /// Set as DataContext for the ConnectionWidget UserControl in StatusBar.xaml.
    /// </summary>
    public ConnectionWidgetViewModel ConnectionWidget { get; }

    // ── Zone A — Safety state ─────────────────────────────────────────────────

    /// <summary>
    /// Whether the Safety Panel drawer is currently open. Drives Zone A button
    /// active/pressed visual state. Status Bar Contract v1.1.0 §2.
    /// </summary>
    [ObservableProperty]
    private bool _isSafetyPanelOpen;

    // ── Connection service state ──────────────────────────────────────────────

    /// <summary>
    /// Effective display connection state for Zone B. Normally mirrors the raw
    /// IConnectionService state, but is promoted to SessionReady when all four
    /// Session Ready conditions are simultaneously satisfied.
    /// </summary>
    [ObservableProperty]
    private ConnectionState _connectionState = ConnectionState.AllDisconnected;

    /// <summary>Total configured device count from IConnectionService.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    private int _deviceCount;

    /// <summary>Number of currently-connected devices from IConnectionService.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    private int _connectedCount;

    /// <summary>Number of devices in an error state from IConnectionService.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    private int _errorCount;

    // ── Safety / sequencer / site state (feeds IsSessionReady) ───────────────

    /// <summary>
    /// Current aggregate safety status from ISafetyService. Session Ready
    /// requires AllClear or NoSource. Named Current* to avoid ambiguity with
    /// the SafetyStatus enum type in computed property expressions.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    [NotifyPropertyChangedFor(nameof(SafetyStatusAutomationLabel))]
    private SafetyStatus _currentSafetyStatus;

    /// <summary>
    /// Whether a valid site profile is configured in Settings → Site &amp; Observatory.
    /// Session Ready requires this to be true.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    private bool _hasValidSiteProfile;

    /// <summary>
    /// Current sequence engine state from ISequencerService. Session Ready requires
    /// Idle or Stopped. Named Current* to avoid ambiguity with the SequencerState enum.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSessionReady))]
    private SequencerState _currentSequencerState;

    // ── Computed: Session Ready ───────────────────────────────────────────────

    /// <summary>
    /// True when all four Session Ready conditions are simultaneously met:
    ///   1. All configured devices connected with zero errors (ErrorCount == 0
    ///      AND ConnectedCount == DeviceCount AND DeviceCount &gt; 0).
    ///   2. Safety status is AllClear or NoSource (no active warnings or unsafe conditions).
    ///   3. A valid site profile is configured (HasValidSiteProfile == true).
    ///   4. Sequencer is in Idle or Stopped state (not actively running a sequence).
    /// This gates the "Start →" CTA in the Connection Widget. Re-evaluated on
    /// every condition change. Status Bar Amendment v1.2.0 §1.
    /// </summary>
    public bool IsSessionReady =>
        // Condition 1: all devices connected, none in error.
        ErrorCount == 0 &&
        ConnectedCount == DeviceCount &&
        DeviceCount > 0 &&
        // Condition 2: no safety warnings — AllClear or NoSource both satisfy the gate.
        (CurrentSafetyStatus == SafetyStatus.AllClear || CurrentSafetyStatus == SafetyStatus.NoSource) &&
        // Condition 3: site profile exists so location-dependent features work.
        HasValidSiteProfile &&
        // Condition 4: sequencer is idle — starting a session while one runs is not allowed.
        (CurrentSequencerState == SequencerState.Idle || CurrentSequencerState == SequencerState.Stopped);

    // ── Computed: Zone A accessibility label ──────────────────────────────────

    /// <summary>
    /// AutomationProperties.Name for the Zone A shield button. Screen readers
    /// announce this so keyboard / AT users know the current safety state without
    /// navigating into the panel.
    /// Status Bar Contract v1.1.0 §2 — Accessibility.
    /// </summary>
    public string SafetyStatusAutomationLabel => CurrentSafetyStatus switch
    {
        SafetyStatus.AllClear      => "Safety Panel \u2014 All Systems Safe",
        SafetyStatus.NoSource      => "Safety Panel \u2014 No Safety Source",
        SafetyStatus.WarningActive => "Safety Panel \u2014 Warning Active",
        SafetyStatus.Unsafe        => "Safety Panel \u2014 Unsafe Conditions",
        _                          => "Safety Panel",
    };

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Toggles the Safety Panel drawer. Bound to the Zone A shield button.
    /// Status Bar Contract v1.1.0 §2.
    /// </summary>
    [RelayCommand]
    private void OpenSafetyPanel() => IsSafetyPanelOpen = !IsSafetyPanelOpen;

    /// <summary>
    /// Navigates the application to Sequencer mode. Invoked by the Zone B
    /// "Start →" chip via ConnectionWidgetViewModel's callback.
    /// Status Bar Amendment v1.2.0 §2.4.
    /// </summary>
    [RelayCommand]
    private void NavigateToSequencer()
    {
        // TODO(forge): Raise navigation event or call NavigationService when available.
        // Navigation architecture is pending App Shell contract.
    }

    // ── Partial property hooks (CommunityToolkit.Mvvm) ────────────────────────

    // DeviceCount, ConnectedCount, and ErrorCount are always updated together from
    // OnConnectionServiceChanged, which calls UpdateEffectiveConnectionState ONCE
    // at the end. Calling it from each partial hook would trigger intermediate
    // evaluations with partially-updated device counts — causing spurious state
    // transitions and unnecessary animation triggers. Only the service-specific
    // conditions (safety, sequencer, site profile) need individual hooks because
    // each service event updates only one property at a time.

    partial void OnCurrentSafetyStatusChanged(SafetyStatus value)      => UpdateEffectiveConnectionState();
    partial void OnHasValidSiteProfileChanged(bool value)              => UpdateEffectiveConnectionState();
    partial void OnCurrentSequencerStateChanged(SequencerState value)  => UpdateEffectiveConnectionState();

    // ── Internal state management ─────────────────────────────────────────────

    /// <summary>
    /// Recomputes the effective ConnectionState from raw service data and the
    /// Session Ready signal, then notifies ConnectionWidgetViewModel.
    /// Called after any condition affecting either value changes.
    /// </summary>
    private void UpdateEffectiveConnectionState()
    {
        ConnectionState effectiveState = IsSessionReady
            ? ConnectionState.SessionReady
            : _connectionService.ConnectionState;

        ConnectionState = effectiveState;

        ConnectionWidget.UpdateState(
            effectiveState,
            ConnectedCount,
            DeviceCount,
            ErrorCount);
    }

    // ── Service event handlers ────────────────────────────────────────────────

    private void OnConnectionServiceChanged(object? sender, EventArgs e)
    {
        // Update all device counts from the service, then re-evaluate state.
        DeviceCount    = _connectionService.DeviceCount;
        ConnectedCount = _connectionService.ConnectedCount;
        ErrorCount     = _connectionService.ErrorCount;
        // All three properties are now updated; evaluate the effective state exactly once.
        UpdateEffectiveConnectionState();
    }

    private void OnSafetyServiceChanged(object? sender, EventArgs e)
        => CurrentSafetyStatus = _safetyService.SafetyStatus;

    private void OnSequencerServiceChanged(object? sender, EventArgs e)
        => CurrentSequencerState = _sequencerService.SequencerState;

    private void OnSiteProfileServiceChanged(object? sender, EventArgs e)
        => HasValidSiteProfile = _siteProfileService.HasValidSiteProfile;

    // ── IDisposable ───────────────────────────────────────────────────────────

    /// <summary>
    /// Unsubscribes from all service events to prevent memory leaks when the
    /// Status Bar ViewModel is replaced (e.g., during hot-reload or test teardown).
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _connectionService.ConnectionStateChanged  -= OnConnectionServiceChanged;
        _safetyService.SafetyStatusChanged         -= OnSafetyServiceChanged;
        _sequencerService.SequencerStateChanged    -= OnSequencerServiceChanged;
        _siteProfileService.SiteProfileChanged     -= OnSiteProfileServiceChanged;
    }
}
