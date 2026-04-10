// src/AstroOS.UI/Services/Interfaces/ISequencerService.cs
using AstroOS.UI.Models;

namespace AstroOS.UI.Services.Interfaces;

/// <summary>
/// Exposes the current state of the sequence engine. Used by StatusBarViewModel
/// to enforce the Session Ready condition: a sequence must not be actively running
/// before the user can enter a new session.
/// Status Bar Amendment v1.2.0 §1.
/// </summary>
public interface ISequencerService
{
    /// <summary>Current execution state of the sequence engine.</summary>
    SequencerState SequencerState { get; }

    /// <summary>
    /// Raised on the UI thread whenever SequencerState changes.
    /// Consumers must not block this event handler.
    /// </summary>
    event EventHandler SequencerStateChanged;
}
