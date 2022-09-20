using ApacheTech.VintageMods.FluentChatCommands.Abstractions;
using ApacheTech.VintageMods.FluentChatCommands.Extensions;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Implementations
{
#if UNIT_TEST
    internal class FluentChatCommand : FluentChatCommandBase, IFluentChatCommand
#else
    internal sealed class FluentChatCommand : FluentChatCommandBase, IFluentChatCommand
#endif
    {
        internal ChatCommand Command { get; }

        internal FluentChatCommand(ChatCommand command, ICoreAPI api) : base(api)
        {
            Command = command;
            api.RunOneOf(
                c => ((ClientChatCommand)command).handler = (id, args) => Handle(c.World.Player, id, args),
                _ => ((ServerChatCommand)command).handler = Handle);
        }

        internal override void DefaultCallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            UApi.ShowChatNotification(player, groupId, Command.GetHelpMessage());
        }

        /// <inheritdoc />
        public IFluentChatCommand WithDescription(string description)
        {
            Command.Description = description;
            return this;
        }

        /// <inheritdoc />
        public IFluentChatCommand RequiresPrivilege(string privilege)
        {
            Command.RequiredPrivilege = privilege.ToLowerInvariant();
            return this;
        }

        /// <inheritdoc />
        public IFluentChatCommand WithHandler(FluentChatCommandHandler handler)
        {
            if (!CallHandler.Method.Name.Equals(nameof(DefaultCallHandler))) return this;
            CallHandler = handler;
            return this;
        }

        /// <inheritdoc />
        public IFluentChatCommand HasSubCommand(string subCommandName, FluentChatSubCommandFactory<IFluentChatCommand> builder)
        {
            var subCommand = new FluentChatSubCommand<IFluentChatCommand>(subCommandName.ToLowerInvariant(), this, UApi);
            builder(subCommand);
            Command.Syntax = GetSyntax();
            return this;
        }
    }
}