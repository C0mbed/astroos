// tests/AstroOS.UI.Tests/ViewModels/StatusBarViewModelTests.cs
//
// Status Bar ViewModel test stubs — for Crucible to implement.
// All methods throw NotImplementedException. Do NOT modify method signatures.
// Add mocks, arrange/act/assert, and remove the throw once Crucible implements.
//
// Conventions:
//   - Use Moq to mock IConnectionService, ISafetyService, ISequencerService, ISiteProfileService.
//   - Each test should be fully self-contained (no shared mutable state between tests).
//   - IsSessionReady is a computed property; tests must drive it via service mock state,
//     not by setting it directly.
//
// Status Bar Contract v1.1.0 + Amendment v1.2.0.
using Xunit;

namespace AstroOS.UI.Tests.ViewModels;

/// <summary>
/// Unit tests for StatusBarViewModel.IsSessionReady and reactive connection state.
/// Crucible will implement all test bodies. Method names are the acceptance criteria.
/// </summary>
public sealed class StatusBarViewModelTests
{
    /// <summary>
    /// IsSessionReady returns true when all four conditions are simultaneously met:
    ///   1. All devices connected, zero errors, at least one device configured.
    ///   2. Safety status is AllClear.
    ///   3. Site profile is valid.
    ///   4. Sequencer is Idle.
    /// Status Bar Amendment v1.2.0 §1.
    /// </summary>
    [Fact]
    public void IsSessionReady_AllConditionsMet_ReturnsTrue()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady returns false when DeviceCount == 0 (no devices configured),
    /// even if all other conditions would be satisfied. You cannot be "ready" with
    /// zero devices.
    /// </summary>
    [Fact]
    public void IsSessionReady_NoDevicesConnected_ReturnsFalse()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady returns false when ErrorCount > 0, regardless of ConnectedCount.
    /// An error device blocks Session Ready — user must retry before imaging.
    /// </summary>
    [Fact]
    public void IsSessionReady_DeviceError_ReturnsFalse()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady returns false when SafetyStatus is WarningActive.
    /// AllClear and NoSource both permit Session Ready; WarningActive and Unsafe block it.
    /// Status Bar Amendment v1.2.0 §1 condition 2.
    /// </summary>
    [Fact]
    public void IsSessionReady_SafetyWarningActive_ReturnsFalse()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady returns false when SequencerState is Running.
    /// A sequence cannot be started while another is already executing.
    /// Status Bar Amendment v1.2.0 §1 condition 4.
    /// </summary>
    [Fact]
    public void IsSessionReady_SequencerRunning_ReturnsFalse()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady returns false when HasValidSiteProfile is false.
    /// Location data is required for altitude limits, meridian flip planning, and plate-solving.
    /// Status Bar Amendment v1.2.0 §1 condition 3.
    /// </summary>
    [Fact]
    public void IsSessionReady_NoSiteProfile_ReturnsFalse()
        => throw new NotImplementedException();

    /// <summary>
    /// IsSessionReady transitions from true to false reactively when any condition
    /// becomes unsatisfied mid-session (e.g., a device disconnects, safety warning fires).
    /// Session Ready clears immediately per Amendment v1.2.0 §1: "Session Ready clears
    /// immediately if any condition above is no longer met."
    /// </summary>
    [Fact]
    public void IsSessionReady_TransitionsToFalse_WhenConditionLost()
        => throw new NotImplementedException();

    /// <summary>
    /// ConnectionState on StatusBarViewModel updates reactively when IConnectionService
    /// raises ConnectionStateChanged. The ViewModel must not poll — it subscribes.
    /// </summary>
    [Fact]
    public void ConnectionState_ChangesReactively_WhenDeviceConnects()
        => throw new NotImplementedException();
}
