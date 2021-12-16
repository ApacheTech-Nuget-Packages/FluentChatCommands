using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.FluentChatCommands.Client;
using ApacheTech.VintageMods.FluentChatCommands.Server;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ApacheTech.VintageMods.FluentChatCommands.Tests.Unit")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

namespace ApacheTech.VintageMods.FluentChatCommands
{
    /// <summary>
    ///     The FluentChat class allows you to build chat commands, for both the server, and the client, using the Fluent
    ///     Builder Pattern.
    /// </summary>
    public static class FluentChat
    {
        internal static readonly Dictionary<string, FluentClientCommand> CachedClientCommands = new();

        internal static readonly Dictionary<string, FluentServerCommand> CachedServerCommands = new();

        /// <summary>
        ///     Returns an instance of <see cref="IFluentClientCommand" />, that can be used to build a client-side command.
        /// </summary>
        /// <param name="commandName">
        ///     The command to be entered by the user. Do not add the leading ".", this will be added
        ///     automatically.
        /// </param>
        public static IFluentClientCommand ClientCommand(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("Invalid command name. Cannot be null, empty, or whitespace.");

            if (CachedClientCommands.ContainsKey(commandName))
                return CachedClientCommands[commandName];

            var command = new FluentClientCommand(commandName);
            CachedClientCommands.Add(commandName, command);
            return command;
        }

        /// <summary>
        ///     Returns an instance of <see cref="IFluentServerCommand" />, that can be used to build a server-side command.
        /// </summary>
        /// <param name="commandName">
        ///     The command to be entered by the user. Do not add the leading "/", this will be added
        ///     automatically.
        /// </param>
        public static IFluentServerCommand ServerCommand(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("Invalid command name. Cannot be null, empty, or whitespace.");

            if (CachedServerCommands.ContainsKey(commandName))
                return CachedServerCommands[commandName];

            var command = new FluentServerCommand(commandName);
            CachedServerCommands.Add(commandName, command);
            return command;
        }

        /// <summary>
        ///     Disposes all client commands.
        /// </summary>
        public static void DisposeClientCommands()
        {
            foreach (var command in CachedClientCommands.Values)
            {
                command.Dispose();
            }
            CachedClientCommands.Clear();
        }

        /// <summary>
        ///     Disposes all server commands.
        /// </summary>
        public static void DisposeServerCommands()
        {
            foreach (var command in CachedServerCommands.Values)
            {
                command.Dispose();
            }
            CachedServerCommands.Clear();
        }

#if DEBUG
        internal static int ClientCommandCount => CachedClientCommands.Count;
        internal static int ServerCommandCount => CachedServerCommands.Count;
#endif
    }
}