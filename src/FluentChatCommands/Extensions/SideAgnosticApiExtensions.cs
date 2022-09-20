using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Extensions
{
    /// <summary>
    ///     Extension methods to aid selective routing between client and server APIs.
    /// </summary>
    public static class SideAgnosticApiExtensions
    {
        /// <summary>
        ///     Chooses between one of two objects, based on whether it's being called by the client, or the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api">The api in question.</param>
        /// <param name="clientObject">The client object.</param>
        /// <param name="serverObject">The server object.</param>
        /// <returns>
        ///     Returns <paramref name="clientObject"/> if called from the client, or <paramref name="serverObject"/> if called from the server.
        /// </returns>
        public static T ChooseOneOf<T>(this ICoreAPI api, T clientObject, T serverObject)
        {
            return api.Side.IsClient() ? clientObject : serverObject;
        }

        /// <summary>
        ///     Invokes an action, based on whether it's being called by the client, or the server.
        /// </summary>
        /// <param name="api">The api in question.</param>
        /// <param name="clientAction">The client action.</param>
        /// <param name="serverAction">The server action.</param>
        public static void RunOneOf(this ICoreAPI api,
            Action<ICoreClientAPI> clientAction,
            Action<ICoreServerAPI> serverAction)
        {
            if (api.Side.IsClient())
            {
                clientAction((api as ICoreClientAPI)!);
                return;
            }
            serverAction((api as ICoreServerAPI)!);
        }

        /// <summary>
        ///     Invokes an action, based on whether it's being called by the client, or the server.
        /// </summary>
        /// <param name="api">The api in question.</param>
        /// <param name="clientAction">The client action.</param>
        /// <param name="serverAction">The server action.</param>
        public static T ReturnOneOf<T>(this ICoreAPI api,
            System.Func<ICoreClientAPI, T> clientAction,
            System.Func<ICoreServerAPI, T> serverAction)
        {
            return api.Side.IsClient()
                ? clientAction((api as ICoreClientAPI)!)
                : serverAction((api as ICoreServerAPI)!);
        }
    }
}