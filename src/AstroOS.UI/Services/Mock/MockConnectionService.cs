// src/AstroOS.UI/Services/Mock/MockConnectionService.cs
// DEBUG only — never registered in release builds.
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;
using Microsoft.UI.Dispatching;

namespace AstroOS.UI.Services.Mock;

/// <summary>
/// Mock IConnectionService for visual verification in DEBUG builds.
/// Cycles through all six ConnectionState values on a DispatcherQueueTimer,
/// advancing one state every two seconds so every visual state in the
/// Connection Widget can be observed without real hardware.
///
/// Cycle order:
///   AllDisconnected → Connecting → PartiallyConnected → AllConnected
///   → SessionReady → ErrorPresent → (repeat)
///
/// Note: when the service is in AllConnected or SessionReady steps, and all
/// other mock services are in their default happy state (AllClear / Idle /
/// HasProfile), StatusBarViewModel.IsSessionReady evaluates to true and the
/// effective display state becomes SessionReady for both steps.
/// </summary>
internal sealed class MockConnectionService : IConnectionService
{
    // ── State cycle ───────────────────────────────────────────────────────────

    private static readonly (ConnectionState State, int DeviceCount, int ConnectedCount, int ErrorCount)[] _cycle =
    [
        (ConnectionState.AllDisconnected,    5, 0, 0),
        (ConnectionState.Connecting,         5, 0, 0),
        (ConnectionState.PartiallyConnected, 5, 3, 0),
        (ConnectionState.AllConnected,       5, 5, 0),
        (ConnectionState.SessionReady,       5, 5, 0),
        (ConnectionState.ErrorPresent,       5, 3, 2),
    ];

    private int _stepIndex;
    private readonly DispatcherQueueTimer _timer;

    // ── Constructor ───────────────────────────────────────────────────────────

    /// <summary>
    /// Must be constructed on the UI thread — captures the current
    /// DispatcherQueue and starts the two-second cycling timer.
    /// </summary>
    public MockConnectionService()
    {
        var queue = DispatcherQueue.GetForCurrentThread()
            ?? throw new InvalidOperationException(
                $"{nameof(MockConnectionService)} must be constructed on the UI thread.");

        // Initialise property values for step 0 (AllDisconnected).
        ApplyStep(0);

        _timer = queue.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(2);
        _timer.IsRepeating = true;
        _timer.Tick += (_, _) => Advance();
        _timer.Start();
    }

    // ── IConnectionService ────────────────────────────────────────────────────

    public ConnectionState ConnectionState { get; private set; }
    public int DeviceCount    { get; private set; }
    public int ConnectedCount { get; private set; }
    public int ErrorCount     { get; private set; }

    public event EventHandler? ConnectionStateChanged;

    public Task ConnectAllAsync(CancellationToken ct = default)    => Task.CompletedTask;
    public Task DisconnectAllAsync(CancellationToken ct = default) => Task.CompletedTask;

    // ── Internal ──────────────────────────────────────────────────────────────

    private void Advance()
    {
        _stepIndex = (_stepIndex + 1) % _cycle.Length;
        ApplyStep(_stepIndex);
        ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyStep(int index)
    {
        var (state, deviceCount, connectedCount, errorCount) = _cycle[index];
        ConnectionState = state;
        DeviceCount     = deviceCount;
        ConnectedCount  = connectedCount;
        ErrorCount      = errorCount;
    }
}
