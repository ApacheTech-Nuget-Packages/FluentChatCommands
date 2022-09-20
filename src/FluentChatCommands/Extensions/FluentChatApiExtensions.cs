using System.Collections.Generic;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Extensions
{
    /// <summary>
    ///     Extension methods to aid the registration and retrieval of FluentChat commands, from the api.
    /// </summary>
    public static class FluentChatApiExtensions
    {
        /// <summary>
        ///     Returns an instance of <see cref="IFluentChatCommand" />, that can be used to build a chat command for the given app-side.
        /// </summary>
        /// <param name="commandName">
        ///     The command to be entered by the user. Do not add the leading "." or "/", this will be added automatically.
        /// </param>
        /// <param name="api">The api to register the command with. Determines whether to register a client, or server command.</param>
        public static IFluentChatCommand? RegisterFluentCommand(this ICoreAPI api, string commandName)
        {
            return FluentChat.RegisterCommand(commandName, api);
        }

        /// <summary>
        ///     Retrieves a command, previously registered with FluentChat, on the current app-side.
        /// </summary>
        /// <param name="api">The API used to call this method.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>A <see cref="IFluentChatCommand"/> representation of the chat command.</returns>
        /// <exception cref="KeyNotFoundException">[FluentChat] No command with the name, '{commandName}', has been registered on the current app-side.</exception>
        public static IFluentChatCommand? FluentCommand(this ICoreAPI api, string commandName)
        {
            return api.ReturnOneOf(
                _ => FluentChat.ClientCommand(commandName),
                _ => FluentChat.ServerCommand(commandName));
        }
    }
}