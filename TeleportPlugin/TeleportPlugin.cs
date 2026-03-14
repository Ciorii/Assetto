using System.Numerics;
using AssettoServer.Commands;
using AssettoServer.Commands.Attributes;
using AssettoServer.Network.Tcp;
using AssettoServer.Shared.Network.Packets.Shared;
using JetBrains.Annotations;
using Qmmands;

namespace TeleportPlugin;

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

        if (target.SessionId == requester.SessionId)
        {
            Reply("[Teleport] Nu te poti teleporta la tine insuti.");
            return;
        }

        var targetStatus = target.EntryCar.Status;
        var requesterCar = requester.EntryCar;

        // Offset on X axis so cars don't perfectly overlap
        requesterCar.Status.Position = targetStatus.Position
            + new Vector3(_configuration.SpawnOffsetMeters, 0f, 0f);
        requesterCar.Status.Rotation = targetStatus.Rotation;
        requesterCar.Status.Velocity = Vector3.Zero;

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
