using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    /// <summary>
    ///     Handles calls to this sub-command. 
    /// </summary>
    /// <param name="subCommandName">The name of the sub-command being handled.</param>
    /// <param name="player">The player that made the call to this command.</param>
    /// <param name="groupId">The chat group identifier.</param>
    /// <param name="args">The arguments passed to the command.</param>
    public delegate void FluentChatServerSubCommandHandler(string subCommandName, IServerPlayer player, int groupId, CmdArgs args);
}