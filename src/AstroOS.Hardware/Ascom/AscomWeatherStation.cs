// src/AstroOS.Hardware/Ascom/AscomWeatherStation.cs
using AstroOS.Hardware.Interfaces;
using Microsoft.Extensions.Logging;

namespace AstroOS.Hardware.Ascom;

/// <summary>
/// TODO: ASCOM ObservingConditions driver implementation (Boltwood, AAG, etc.).
/// </summary>
public sealed class AscomWeatherStation : IWeatherStation
{
    // TODO(forge): Wrap ASCOM.Com.DriverAccess.ObservingConditions with IWeatherStation contract
}
