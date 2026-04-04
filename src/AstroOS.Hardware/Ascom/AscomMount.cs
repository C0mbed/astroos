// src/AstroOS.Hardware/Ascom/AscomMount.cs
using AstroOS.Hardware.Interfaces;
using Microsoft.Extensions.Logging;

namespace AstroOS.Hardware.Ascom;

/// <summary>
/// TODO: ASCOM local server mount driver implementation via ASCOM.Com.DriverAccess.
/// </summary>
public sealed class AscomMount : IMount
{
    // TODO(forge): Wrap ASCOM.Com.DriverAccess.Telescope with IMount contract
}
