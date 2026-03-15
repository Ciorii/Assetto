using System.Numerics;
using AssettoServer.Network.ClientMessages;

namespace TeleportPlugin.Packets;

[OnlineEvent(Key = "AS_TeleportToPlayer")]
public class TeleportToPlayerPacket : OnlineEvent<TeleportToPlayerPacket>
{
    [OnlineEventField(Name = "position")]
    public Vector3 Position;

    [OnlineEventField(Name = "direction")]
    public Vector3 Direction;
}
