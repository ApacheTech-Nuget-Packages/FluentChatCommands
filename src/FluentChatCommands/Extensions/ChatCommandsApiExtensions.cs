using System;
using System.Collections.Generic;
using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Extensions
{
    /// <summary>
    ///     Extension methods to aid working with Chat Commands, with the API.  
    /// </summary>
    public static class ChatCommandsApiExtensions
    {
        /// <summary>
        ///     Determines whether any chat command with the specified name has been registered with the current app-side.
        /// </summary>
        /// <param name="api">The API used to call this method.</param>
        /// <param name="commandName">The name of the command to look for.</param>
        /// <returns><c>true</c> if the command is registered; otherwise, <c>false</c>.</returns>
        public static bool IsCommandRegistered(this ICoreAPI api, string commandName)
        {
            return api.RegisteredCommands()
                .ContainsKey(commandName.ToLowerInvariant());
        }

        /// <summary>
        ///     Gathers a list of commands that have been registered with the API.
        /// </summary>
        /// <param name="capi">The client API used to call this method.</param>
        /// <returns>A shallow copy of the `chatCommands` internal dictionary that the game uses to store registered chat commands.</returns>
        public static Dictionary<string, ChatCommand> RegisteredCommands(this ICoreClientAPI capi)
        {
            var eventManager = capi.World.GetField<ClientEventManager>("eventManager");
            var commands = eventManager.GetField<Dictionary<string, ChatCommand>>("chatCommands");
            return commands;
        }

        /// <summary>
        ///     Gathers a list of commands that have been registered with the API.
        /// </summary>
        /// <param name="sapi">The server API used to call this method.</param>
        /// <returns>A shallow copy of the `chatCommands` internal dictionary that the game uses to store registered chat commands.</returns>
        public static Dictionary<string, ChatCommand> RegisteredCommands(this ICoreServerAPI sapi)
        {
            var eventManager = sapi.World.GetField<ServerEventManager>("ModEventManager");
            var commands = eventManager.GetField<Dictionary<string, ChatCommand>>("chatCommands");
            return commands;
        }

        /// <summary>
        ///     Gathers a list of commands that have been registered with the API.
        /// </summary>
        /// <param name="api">The API used to call this method.</param>
        /// <returns>A shallow copy of the `chatCommands` internal dictionary that the game uses to store registered chat commands.</returns>
        public static Dictionary<string, ChatCommand> RegisteredCommands(this ICoreAPI api)
        {
            return api.ReturnOneOf(
                c => c.RegisteredCommands(),
                s => s.RegisteredCommands());
        }

        /// <summary>
        ///     Unregisters a command on the client.
        /// </summary>
        /// <param name="capi">The API used to call this method.</param>
        /// <param name="commandName">The name of the command.</param>
        public static void UnregisterCommand(this ICoreClientAPI capi, string commandName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Unregisters a command on the server.
        /// </summary>
        /// <param name="sapi">The API used to call this method.</param>
        /// <param name="commandName">The name of the command.</param>
        public static void UnregisterCommand(this ICoreServerAPI sapi, string commandName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Unregisters a command on the current app-side.
        /// </summary>
        /// <param name="api">The API used to call this method.</param>
        /// <param name="commandName">The name of the command.</param>
        public static void UnregisterCommand(this ICoreAPI api, string commandName)
        {
            api.RunOneOf(
                c => c.UnregisterCommand(commandName),
                s => s.UnregisterCommand(commandName));
        }
    }
}