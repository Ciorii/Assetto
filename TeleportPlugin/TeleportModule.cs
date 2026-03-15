using Autofac;
using AssettoServer.Server.Plugin;
using JetBrains.Annotations;

namespace TeleportPlugin;

/// <summary>
/// Registers TeleportPlugin services with AssettoServer's DI container.
/// AssettoServer discovers this class automatically via reflection.
/// </summary>
[UsedImplicitly]
public class TeleportModule : AssettoServerModule<TeleportConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        // TeleportCommandModule (ACModuleBase) is discovered automatically
        // by ChatService via _commandService.AddModules(plugin.Assembly).
        builder.RegisterType<TeleportAutostart>()
            .AsSelf()
            .As<IAssettoServerAutostart>()
            .SingleInstance();
    }
}
