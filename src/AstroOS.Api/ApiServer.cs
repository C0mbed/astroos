// src/AstroOS.Api/ApiServer.cs
using AstroOS.Core.Events;
using Microsoft.Extensions.Logging;

namespace AstroOS.Api;

/// <summary>
/// TODO: REST + WebSocket API server for companion app and remote control.
/// Hosts ASP.NET Core minimal API within the WinUI process.
/// </summary>
public sealed class ApiServer
{
    // TODO(forge): Implement ASP.NET Core hosted service, REST endpoints for device state,
    //              WebSocket channel for live telemetry streaming
}
