using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.FluentChatCommands.Extensions;
using ApacheTech.VintageMods.FluentChatCommands.Implementations;
using Vintagestory.API.Common;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands
{
    /// <summary>
    ///     The FluentChat class allows you to build chat commands, for both the server, and the client.
    /// </summary>
    public static class FluentChat
    {
        internal static readonly HashSet<FluentChatCommand> CachedCommands = new();

        /// <summary>
        ///     Returns an instance of <see cref="IFluentChatCommand" />, that can be used to build a chat command for the given app-side.
        /// </summary>
        /// <param name="commandName">
        ///     The command to be entered by the user. Do not add the leading "." or "/", this will be added automatically.
        /// </param>
        /// <param name="api">The api to register the command with. Determines whether to register a client, or server command.</param>
        public static IFluentChatCommand? RegisterCommand(string commandName, ICoreAPI api)
        {
            if (api is null) throw new ArgumentNullException(nameof(api));
            if (string.IsNullOrWhiteSpace(commandName))
            {
                api.Logger.Error("[FluentChat] Command name cannot be null, empty, or whitespace.");
                return null;
            }

            commandName = commandName.ToLowerInvariant();
            var command = api.ChooseOneOf<ChatCommand>(
                new ClientChatCommand { Command = commandName, Description = string.Empty },
                new ServerChatCommand { Command = commandName, Description = string.Empty });

            return TryRegisterCommand(api, command);
        }

        /// <summary>
        ///     Retrieves a command, previously registered with FluentChat, on the client.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>A <see cref="IFluentChatCommand"/> representation of the client-side chat command.</returns>
        /// <exception cref="KeyNotFoundException">[FluentChat] No client command with the name, '{commandName}', has been registered.</exception>
        public static IFluentChatCommand ClientCommand(string commandName)
        {
            var command = GetCachedCommand(commandName, EnumAppSide.Client);
            if (command is not null) return command;
            throw new KeyNotFoundException(
                $"[FluentChat] No client command with the name, '{commandName}', has been registered.");
        }

        /// <summary>
        ///     Retrieves a command, previously registered with FluentChat, on the server.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>A <see cref="IFluentChatCommand"/> representation of the server-side chat command.</returns>
        /// <exception cref="KeyNotFoundException">[FluentChat] No server command with the name, '{commandName}', has been registered.</exception>
        public static IFluentChatCommand ServerCommand(string commandName)
        {
            var command = GetCachedCommand(commandName, EnumAppSide.Server);
            if (command is not null) return command;
            throw new KeyNotFoundException(
                $"[FluentChat] No server command with the name, '{commandName}', has been registered.");
        }

        /// <summary>
        ///     Unregisters a command, previously registered with FluentChat, on the server.
        ///     Also unregisters the command with the api.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="api">The api the command was registered with.</param>
        public static void UnregisterCommand(string commandName, ICoreAPI api)
        {
            var command = GetCachedCommand(commandName, api.Side);
            if (command is null) return;
            CachedCommands.Remove(command);
#if UNIT_TEST
            OnUnregisterCommand?.Invoke(commandName);
#else
            api.UnregisterCommand(commandName);
#endif
        }

        /// <summary>
        ///     Unregisters all FluentChat commands that have been registered with the api.
        /// </summary>
        /// <param name="api">The api the commands were registered with.</param>
        [Obsolete("Commands are now cleaned up by the game, when leaving a world.")]
        public static void ClearCommands(ICoreAPI api)
        {
            throw new NotImplementedException();
            var commands = CachedCommands.Where(p => p.Side == api.Side).ToList();
            foreach (var command in commands)
            {
                UnregisterCommand(command.Command.Command, api);
            }
        }

#if UNIT_TEST
        internal static Action<string>? OnUnregisterCommand { get; set; }
#endif

        internal static FluentChatCommand? TryRegisterCommand(ICoreAPI api, ChatCommand command)
        {
            var success = api.ReturnOneOf(
                capi => capi.RegisterCommand(command as ClientChatCommand),
                sapi => sapi.RegisterCommand(command as ServerChatCommand));

            if (!success)
            {
                api.Logger.Error($"[FluentChat] Command '{command.Command}' has already been registered.");
                return null;
            }

            var wrappedCommand = new FluentChatCommand(command, api);
            CachedCommands.Add(wrappedCommand);
            return wrappedCommand;
        }

        private static FluentChatCommand? GetCachedCommand(string commandName, EnumAppSide side)
        {
            return CachedCommands
                .FirstOrDefault(p =>
                    p.Command.Command.Equals(commandName, StringComparison.InvariantCultureIgnoreCase) &&
                    p.Side == side);
        }
    }
}
