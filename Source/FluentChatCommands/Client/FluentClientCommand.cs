using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    internal sealed class FluentClientCommand : IFluentClientCommand
    {
        private ICoreClientAPI _capi;

#if DEBUG
        internal ClientChatCommand ChatCommand { get; }
        internal Dictionary<string, FluentClientSubCommand> SubCommands { get; } = new();
        internal ClientChatCommandDelegate DefaultHandler { get; private set; }

#else
        private ClientChatCommand ChatCommand { get; }
        private Dictionary<string, FluentClientSubCommand> SubCommands { get; } = new();
        private ClientChatCommandDelegate DefaultHandler { get; set; }
#endif

        internal FluentClientCommand(string commandName)
        {
            ChatCommand = new ClientChatCommand
            {
                Command = commandName,
                Description = "",
                handler = CallHandler
            };
        }

        /// <summary>
        ///     Specifies the description for the command.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        public IFluentClientCommand HasDescription(string description)
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
        public IFluentClientCommand HasDefaultHandler(ClientChatCommandDelegate handler)
        {
            DefaultHandler = handler;
            return this;
        }

        /// <summary>
        ///     Registers this command with the client-side API.
        ///     FluentChat supports lazy loading, so this method can be called at any time along the build process.
        /// </summary>
        /// <param name="capi">The client-side API to register the command with.</param>
        /// <returns>System.Boolean.</returns>
        public IFluentClientCommand RegisterWith(ICoreClientAPI capi)
        {
            ChatCommand.Syntax = GetSyntax();
            (_capi = capi).RegisterCommand(ChatCommand);
            return this;
        }

        /// <summary>
        ///     Adds a sub-command, with it's own command handler.
        ///     All sub-commands must have handlers set, or this command will throw a <see cref="OrphanedSubCommandException" />
        ///     exception when called.
        /// </summary>
        /// <param name="subCommandName">Name of the sub command.</param>
        /// <returns>Returns the sub-command, so that a handler can be set.</returns>
        public IFluentClientSubCommand HasSubCommand(string subCommandName)
        {
            if (SubCommands.ContainsKey(subCommandName))
            {
                throw new SubCommandAlreadyExistsException(ChatCommand, $"The sub-command `{subCommandName}` has already been declared for the `.{ChatCommand.Command}` command.");
            }

            var subCommand = new FluentClientSubCommand(this);
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
        internal void CallHandler(int groupId, CmdArgs args)
#else
        private void CallHandler(int groupId, CmdArgs args)
#endif
        {
            foreach (var subCommand in SubCommands.Values.Where(subCommand => subCommand.Handler is null))
            {
                throw new OrphanedSubCommandException(ChatCommand, $"The sub-command `{subCommand}`, for the `.{ChatCommand.Command}` command has no handler set.");
            }

            switch (DefaultHandler)
            {
                case null when !SubCommands.Values.Any(p => p is not null):
                    throw new NoHandlersFoundException(ChatCommand, $"The command `.{ChatCommand.Command}` has no handlers set.");
                case null when args.Length is 0:
                    _capi.ShowChatMessage(ChatCommand.GetHelpMessage());
                    return;
                case not null when args.Length is 0:
                    DefaultHandler(groupId, args);
                    return;
            }

            if (args.Length > 0)
            {
                var firstArg = args.PopWord();
                if (!string.IsNullOrWhiteSpace(firstArg) && SubCommands.ContainsKey(firstArg))
                    SubCommands[firstArg].Handler(firstArg, groupId, args);
            }
            else
            {
                _capi.ShowChatMessage(ChatCommand.GetHelpMessage());
            }
        }
    }
}