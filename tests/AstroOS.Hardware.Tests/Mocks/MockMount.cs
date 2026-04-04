// tests/AstroOS.Hardware.Tests/Mocks/MockMount.cs
using AstroOS.Hardware.Interfaces;

namespace AstroOS.Hardware.Tests.Mocks;

/// <summary>
/// TODO: In-memory mock mount for sequencer and safety system tests.
/// </summary>
public sealed class MockMount : IMount
{
    // TODO(forge): Implement controllable fake mount — configurable latency, error injection,
    //              pier side simulation, and HA limit simulation
}
