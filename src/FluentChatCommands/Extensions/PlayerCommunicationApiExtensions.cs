using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Extensions
{
    /// <summary>
    ///     Extension methods to aid communication with players.
    /// </summary>
    public static class PlayerCommunicationApiExtensions
    {
        /// <summary>
        ///     Shows a chat message in the current chat channel. Uses the same code paths a server/client message takes. Does not execute commands.
        /// </summary>
        /// <param name="api">The API used to call this method.</param>
        /// <param name="player">The player to send the message to.</param>
        /// <param name="groupId">The chat group identifier.</param>
        /// <param name="message">The message to send.</param>
        public static void ShowChatNotification(this ICoreAPI api, IPlayer player, int groupId, string message)
        {
            switch (api)
            {
                case ICoreClientAPI capi:
                    capi.ShowChatMessage(message);
                    return;
                case ICoreServerAPI sapi:
                    sapi.SendMessage(player, groupId, message, EnumChatType.Notification);
                    return;
            }
        }
    }
}