// src/AstroOS.UI/Services/Mock/MockSequencerService.cs
// DEBUG only — never registered in release builds.
using AstroOS.UI.Models;
using AstroOS.UI.Services.Interfaces;

namespace AstroOS.UI.Services.Mock;

/// <summary>
/// Mock ISequencerService for visual verification in DEBUG builds.
/// Always reports Idle and never fires SequencerStateChanged,
/// satisfying the sequencer condition of the Session Ready gate unconditionally.
/// </summary>
internal sealed class MockSequencerService : ISequencerService
{
    public SequencerState SequencerState => SequencerState.Idle;

    // Event intentionally never raised.
    public event EventHandler? SequencerStateChanged { add { } remove { } }
}
