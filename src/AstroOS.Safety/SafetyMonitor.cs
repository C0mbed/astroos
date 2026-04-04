// src/AstroOS.Safety/SafetyMonitor.cs
using AstroOS.Core.Events;
using Microsoft.Extensions.Logging;

namespace AstroOS.Safety;

/// <summary>
/// TODO: Site safety monitor — polls configured safety source and manages the
/// SAFE / CAUTION / UNSAFE / OVERRIDE state machine.
/// See: pipeline/specs/safety-system-tech-spec.md § Subsystem 3
/// </summary>
public sealed class SafetyMonitor
{
    // TODO(forge): Implement safe-file, ASCOM ObservingConditions, HTTP, and INDI source polling
    //              Signal loss handling: treat silence as UNSAFE after grace period
}
