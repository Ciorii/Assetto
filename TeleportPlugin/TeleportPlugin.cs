using System.Numerics;
using System.Reflection;
using AssettoServer.Commands;
using AssettoServer.Commands.Attributes;
using AssettoServer.Network.ClientMessages;
using AssettoServer.Network.Tcp;
using AssettoServer.Server;
using AssettoServer.Server.Plugin;
using AssettoServer.Shared.Network.Packets.Shared;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Qmmands;
using Serilog;

using TeleportPlugin.Packets;

namespace TeleportPlugin;

/// <summary>
/// Loads the CSP Lua script required for client-side teleport execution.
/// Without this script, server-side position updates are often rejected
/// by the local AC client.
/// </summary>
[UsedImplicitly]
public class TeleportAutostart : IAssettoServerAutostart
{
    public TeleportAutostart(CSPServerScriptProvider scriptProvider)
    {
        var pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var luaPath = Path.Join(pluginDir, "lua", "tp.lua");

        if (luaPath == null || !File.Exists(luaPath))
        {
            Log.Warning("[TeleportPlugin] Lua script not found at {Path}. /tp will not move the local client.", luaPath);
            return;
        }

        scriptProvider.AddScriptFile(luaPath, "tp.lua");
        Log.Information("[TeleportPlugin] Loaded CSP script tp.lua");
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Qmmands command module that handles the /tp chat command.
/// Messages starting with '/' are routed directly to Qmmands by ChatService,
/// so this is the correct way to intercept slash commands in AssettoServer.
/// </summary>
[UsedImplicitly]
public class TeleportCommandModule : ACModuleBase
{
    private readonly TeleportConfiguration _configuration;

    public TeleportCommandModule(TeleportConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// /tp &lt;player&gt; — teleports the caller to the named player.
    /// The ACTcpClient parameter is resolved automatically by ACClientTypeParser
    /// using exact match, then case-insensitive, then partial name match.
    /// </summary>
    [Command("tp"), RequireConnectedPlayer]
    public void Teleport([Remainder] ACTcpClient target)
    {
        var requester = Client!;

        if (requester.CSPVersion == null)
        {
            Reply("[Teleport] Ai nevoie de CSP pentru /tp (client-side teleport).");
            return;
        }

        if (target.SessionId == requester.SessionId)
        {
            Reply("[Teleport] Nu te poti teleporta la tine insuti.");
            return;
        }

        var targetStatus = target.EntryCar.Status;
        var requesterCar = requester.EntryCar;

        var teleportPosition = targetStatus.Position + new Vector3(_configuration.SpawnOffsetMeters, 0f, 0f);
        var teleportDirection = targetStatus.Rotation;

        if (teleportDirection == Vector3.Zero)
            teleportDirection = new Vector3(1f, 0f, 0f);

        // Keep server-side state aligned with the intended teleport target.
        requesterCar.Status.Position = teleportPosition;
        requesterCar.Status.Rotation = teleportDirection;
        requesterCar.Status.Velocity = Vector3.Zero;

        // Trigger actual local teleport on the requester's client via CSP Lua.
        requester.SendPacket(new TeleportToPlayerPacket
        {
            SessionId = 255,
            Position = teleportPosition,
            Direction = teleportDirection
        });

        Reply($"[Teleport] Ai fost teleportat la {target.Name}.");

        if (_configuration.NotifyTarget)
        {
            target.SendPacket(new ChatMessage
            {
                SessionId = 255,
                Message = $"[Teleport] {requester.Name} s-a teleportat la tine."
            });
        }
    }
}
