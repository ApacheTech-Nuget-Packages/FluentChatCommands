using ApacheTech.VintageMods.FluentChatCommands;
using ApacheTech.VintageMods.FluentChatCommands.Extensions;
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
            BuildCommand(capi);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            sapi.RegisterCommand("apache", "", "", (p, i, _) => { sapi.ShowChatNotification(p, i, "Sanity Check"); });
            BuildCommand(sapi);
        }

        private static void BuildCommand(ICoreAPI api)
        {
            api.RegisterFluentCommand("fluent")!
                .WithDescription("This is an integration test for the FluentChat Nuget Package.")
                .WithHandler((p, i, _) => api.ShowChatNotification(p, i, "Default Handler"));

            var command = api.FluentCommand("fluent")!
                .HasSubCommand("sub1", x => x.WithHandler((player, groupId, args) =>
                {
                    api.ShowChatNotification(player, groupId, "Now inside the `sub1` sub-command.");
                    api.ShowChatNotification(player, groupId, $" - Args: {args.PopAll()}");
                }).Build());


            command.HasSubCommand("sub2", x => x
                .WithAlias("subAlias")
                .WithHandler((player, groupId, args) =>
                {
                    api.ShowChatNotification(player, groupId, "Now inside the `sub2` sub-command.");
                    api.ShowChatNotification(player, groupId, $" - Args: {args.PopAll()}");
                }).Build());

            api.FluentCommand("fluent")!
                .HasSubCommand("sub3", x => x.WithHandler((player, groupId, args) =>
                {
                    var newSub = args.PopWord("sub4");
                    api.ShowChatNotification(player, groupId, $"Adding `{newSub}` as a new sub-command at runtime.");
                    api.ShowChatNotification(player, groupId, " - Before:");
                    api.ShowChatNotification(player, groupId, ".help fluent");

                    command.HasSubCommand(newSub, x => x.WithHandler((_, _, newArgs) =>
                    {
                        api.ShowChatNotification(player, groupId, $"Now inside the `{newSub}` sub-command.");
                        api.ShowChatNotification(player, groupId, $" - Args: {newArgs.PopAll()}");
                    }).Build());

                    api.ShowChatNotification(player, groupId, " - After:");
                    api.ShowChatNotification(player, groupId, ".help fluent");
                }).Build());
        }
    }
}
