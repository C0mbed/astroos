// src/AstroOS.Safety/Limits/HorizonMask.cs
namespace AstroOS.Safety.Limits;

/// <summary>
/// TODO: Per-azimuth horizon mask with linear interpolation between user-placed points.
/// See: pipeline/specs/safety-system-tech-spec.md § 1c
/// </summary>
public sealed class HorizonMask
{
    // TODO(forge): Implement point list model, linear interpolation, Cartes du Ciel .hrz import,
    //              and pre-slew altitude-vs-azimuth check
}
