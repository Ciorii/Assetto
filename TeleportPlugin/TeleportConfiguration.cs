using JetBrains.Annotations;

namespace TeleportPlugin;

/// <summary>
/// Configuration for TeleportPlugin, loaded from cfg/teleport_plugin.yml
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class TeleportConfiguration
{
    /// <summary>
    /// Offset in meters applied on the X axis so the teleported car
    /// doesn't perfectly overlap the target car.
    /// Default: 3.0 metres
    /// </summary>
    public float SpawnOffsetMeters { get; init; } = 3.0f;

    /// <summary>
    /// Whether the target player should receive a chat notification
    /// when someone teleports to them.
    /// Default: true
    /// </summary>
    public bool NotifyTarget { get; init; } = true;


}
