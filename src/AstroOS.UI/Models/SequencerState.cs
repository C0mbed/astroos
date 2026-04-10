// src/AstroOS.UI/Models/SequencerState.cs
namespace AstroOS.UI.Models;

/// <summary>
/// Current state of the sequence engine. Idle and Stopped satisfy the
/// Session Ready gate; Running and Paused do not — Session Ready requires
/// the sequencer to not be in active execution.
/// Status Bar Amendment v1.2.0 §1.
/// </summary>
public enum SequencerState
{
    /// <summary>No sequence loaded or sequencer waiting for input. Session Ready gate: satisfied.</summary>
    Idle,

    /// <summary>A sequence is actively executing. Session Ready gate: NOT satisfied.</summary>
    Running,

    /// <summary>
    /// Sequence execution finished or was halted by the user.
    /// Session Ready gate: satisfied (rig is still configured and ready).
    /// </summary>
    Stopped,

    /// <summary>Sequence is temporarily suspended mid-run. Session Ready gate: NOT satisfied.</summary>
    Paused,
}
