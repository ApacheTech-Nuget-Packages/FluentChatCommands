using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable UnusedType.Global

namespace FluentChatCommands.Tests.Integration
{
    /// <summary>
    ///     The purpose of this class is ensure that end-to-end testing can be performed on the FluentChat
    ///     package within a test-world environment. A sanity check command has been added, ".apache" so
    ///     that it's easy to see whether the mod has made its way into the world without error.
    ///
    ///     As new features are added, this class may need to be extended, or re-purposed for use. It can
    ///     also be used to create working examples for code mod documentation.
    /// </summary>>
    internal class IntegrationsTest : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide forSide) => true;

        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.RegisterCommand("apache", "", "", (_, _) => { capi.ShowChatMessage("Sanity Check"); });

            FluentChat.ClientCommand("fluent")
                .RegisterWith(capi)
                .HasDescription("This is an integration test for the FluentChat Nuget Package.")
                .HasDefaultHandler((_, _) => capi.ShowChatMessage("Default Handler"));

            var command = FluentChat.ClientCommand("fluent")
                .HasSubCommand("sub1")
                .WithHandler((name, _, args) =>
                {
                    capi.ShowChatMessage($"Now inside the `{name}` sub-command.");
                    capi.ShowChatMessage($" - Args: {args.PopAll()}");
                });

            command.HasSubCommand("sub2")
                .WithHandler((name, _, args) =>
                {
                    capi.ShowChatMessage($"Now inside the `{name}` sub-command.");
                    capi.ShowChatMessage($" - Args: {args.PopAll()}");
                });

            FluentChat.ClientCommand("fluent")
                .HasSubCommand("sub3")
                .WithHandler((_, _, args) =>
                {
                    var newSub = args.PopWord("sub4");
                    capi.ShowChatMessage($"Adding `{newSub}` as a new sub-command at runtime.");
                    capi.ShowChatMessage(" - Before:");
                    capi.TriggerChatMessage(".help fluent");

                    command.HasSubCommand(newSub).WithHandler((newName, _, newArgs) =>
                    {
                        capi.ShowChatMessage($"Now inside the `{newName}` sub-command.");
                        capi.ShowChatMessage($" - Args: {newArgs.PopAll()}");
                    });

                    capi.ShowChatMessage(" - After:");
                    capi.TriggerChatMessage(".help fluent");
                });
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            FluentChat.ServerCommand("fluent")
                .RegisterWith(sapi)
                .HasDescription("This is an integration test for the FluentChat Nuget Package.")
                .HasDefaultHandler((player, groupId, _) => sapi.SendMessage(player, groupId, "Default Handler", EnumChatType.Notification));


            var command = FluentChat.ServerCommand("fluent")
                .HasSubCommand("sub1")
                .WithHandler((name, player, groupId, args) =>
                {
                    sapi.SendMessage(player, groupId, $"Now inside the `{name}` sub-command.", EnumChatType.Notification);
                    sapi.SendMessage(player, groupId, $" - Args: {args.PopAll()}", EnumChatType.Notification);
                });

            command.HasSubCommand("sub2")
                .WithHandler((name, player, groupId, args) =>
                {
                    sapi.SendMessage(player, groupId, $"Now inside the `{name}` sub-command.", EnumChatType.Notification);
                    sapi.SendMessage(player, groupId, $" - Args: {args.PopAll()}", EnumChatType.Notification);
                });

            FluentChat.ServerCommand("fluent")
                .HasSubCommand("sub3")
                .WithHandler((_, player, groupId, args) =>
                {
                    var newSub = args.PopWord("sub4");
                    sapi.SendMessage(player, groupId, $"Adding `{newSub}` as a new sub-command at runtime.", EnumChatType.Notification);
                    sapi.SendMessage(player, groupId, " - Before:", EnumChatType.Notification);

                    sapi.InjectConsole("/help fluent");

                    command.HasSubCommand(newSub).WithHandler((newName, _, _, newArgs) =>
                    {
                        sapi.SendMessage(player, groupId, $"Now inside the `{newName}` sub-command.", EnumChatType.Notification);
                        sapi.SendMessage(player, groupId, $" - Args: {newArgs.PopAll()}", EnumChatType.Notification);
                    });

                    sapi.SendMessage(player, groupId, " - After:", EnumChatType.Notification);
                    sapi.InjectConsole("/help fluent");
                });
        }
    }
}
