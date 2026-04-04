// src/AstroOS.Safety/GoodnightSequence.cs
using AstroOS.Core.Events;
using Microsoft.Extensions.Logging;

namespace AstroOS.Safety;

/// <summary>
/// TODO: Goodnight sequence engine — ordered shutdown steps with enable/disable per step.
/// See: pipeline/specs/safety-system-tech-spec.md § Subsystem 2
/// </summary>
public sealed class GoodnightSequence
{
    // TODO(forge): Implement 8-step shutdown sequence (graceful and immediate modes)
    //              Steps: FinishExposure, WarmCamera, ParkMount, CloseFlatPanel,
    //                     CloseDome, DisconnectDevices, RestState, CustomScript
}
