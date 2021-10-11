using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    /// <summary>
    ///     Handles calls to this sub-command. 
    /// </summary>
    /// <param name="subCommandName">The name of the sub-command being handled.</param>
    /// <param name="groupId">The chat group identifier.</param>
    /// <param name="args">The arguments passed to the command.</param>
    public delegate void FluentChatClientSubCommandHandler(string subCommandName, int groupId, CmdArgs args);
}