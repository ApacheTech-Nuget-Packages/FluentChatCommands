using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ApacheTech.VintageMods.FluentChatCommands.Tests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    internal sealed class FluentServerCommand : IFluentServerCommand
    {
        private ICoreServerAPI _sapi;

#if DEBUG
        internal ServerChatCommand ChatCommand { get; }
        internal Dictionary<string, FluentServerSubCommand> SubCommands { get; } = new();
        internal ServerChatCommandDelegate DefaultHandler { get; private set; }

#else
        private ServerChatCommand ChatCommand { get; }
        private Dictionary<string, FluentServerSubCommand> SubCommands { get; } = new();
        private ServerChatCommandDelegate DefaultHandler { get; set; }
#endif

        internal FluentServerCommand(string commandName)
        {
            ChatCommand = new ServerChatCommand
            {
                Command = commandName,
                Description = "",
                RequiredPrivilege = null,
                handler = CallHandler
            };
        }

        /// <summary>
        ///     Specifies the required privilege a user must have, before the can use this command.
        /// </summary>
        /// <param name="privilege">The privilege to set.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        public IFluentServerCommand RequiresPrivilege(string privilege)
        {
            if (!Privilege.AllCodes().Contains(privilege))
                throw new PrivilegeMismatchException(ChatCommand, $"An invalid privilege name, `{privilege}`, has been passed to the command, `/{ChatCommand.Command}`");

            if (!string.IsNullOrWhiteSpace(ChatCommand.RequiredPrivilege))
                throw new PrivilegeOverrideException(ChatCommand, $"The command `/{ChatCommand.Command}` already has a privilege set.", privilege);

            ChatCommand.RequiredPrivilege = privilege;
            return this;
        }

        /// <summary>
        ///     Specifies the description for the command.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        public IFluentServerCommand HasDescription(string description)
        {
            ChatCommand.Description = description;
            return this;
        }

        /// <summary>
        ///     Specifies the default command handler to use for this command.
        ///     If no default handler is set, sub-commands will still function, and calling the command with no arguments will show
        ///     the command's help message to the user.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        public IFluentServerCommand HasDefaultHandler(ServerChatCommandDelegate handler)
        {
            DefaultHandler = handler;
            return this;
        }

        /// <summary>
        ///     Registers this command with the server-side API.
        ///     FluentChat supports lazy loading, so this method can be called at any time along the build process.
        /// </summary>
        /// <param name="sapi">The server-side API to register the command with.</param>
        /// <returns>System.Boolean.</returns>
        public IFluentServerCommand RegisterWith(ICoreServerAPI sapi)
        {
            ChatCommand.Syntax = GetSyntax();
            (_sapi = sapi).RegisterCommand(ChatCommand);
            return this;
        }

        /// <summary>
        ///     Adds a sub-command, with it's own command handler.
        ///     All sub-commands must have handlers set, or this command will throw a <see cref="OrphanedSubCommandException" />
        ///     exception when called.
        /// </summary>
        /// <param name="subCommandName">Name of the sub command.</param>
        /// <returns>Returns the sub-command, so that a handler can be set.</returns>
        public IFluentServerSubCommand HasSubCommand(string subCommandName)
        {
            if (SubCommands.ContainsKey(subCommandName))
            {
                throw new SubCommandAlreadyExistsException(ChatCommand, $"The sub-command `{subCommandName}` has already been declared for the `/{ChatCommand.Command}` command.");
            }

            var subCommand = new FluentServerSubCommand(this);
            SubCommands.Add(subCommandName, subCommand);
            ChatCommand.Syntax = GetSyntax();
            return subCommand;
        }

        private string GetSyntax()
        {
            return
                SubCommands.Count > 0
                    ? $"[{string.Join<string>("|", SubCommands.Keys.ToArray())}]"
                    : "";
        }

#if DEBUG
        internal void CallHandler(IServerPlayer player, int groupId, CmdArgs args)
#else

        private void CallHandler(IServerPlayer player, int groupId, CmdArgs args)
#endif
        {
            foreach (var subCommand in SubCommands.Values.Where(subCommand => subCommand.Handler is null))
            {
                throw new OrphanedSubCommandException(ChatCommand, $"The sub-command `{subCommand}`, for the `/{ChatCommand.Command}` command has no handler set.");
            }

            switch (DefaultHandler)
            {
                case null when !SubCommands.Values.Any(p => p is not null):
                    throw new NoHandlersFoundException(ChatCommand, $"The command `/{ChatCommand.Command}` has no handlers set.");
                case null when args.Length is 0:
                    _sapi.SendMessage(player, groupId, ChatCommand.GetHelpMessage(), EnumChatType.Notification);
                    return;
                case not null when args.Length is 0:
                    DefaultHandler(player, groupId, args);
                    return;
            }
        
            var firstArg = args.PopWord();
            if (!string.IsNullOrWhiteSpace(firstArg) && SubCommands.ContainsKey(firstArg))
            {
                SubCommands[firstArg].Handler(firstArg, player, groupId, args);
            }
            else
            {
                _sapi.SendMessage(player, groupId, ChatCommand.GetHelpMessage(), EnumChatType.Notification);
            }
        }
    }
}