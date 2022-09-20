using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Abstractions
{
    /// <summary>
    ///     A side-agnostic handler for chat commands. When used on the client, the current
    ///     <see cref="IClientPlayer"/> will be passed to the <paramref name="player"/> parameter.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="groupId">The group identifier.</param>
    /// <param name="args">The arguments.</param>
    public delegate void FluentChatCommandHandler(IPlayer player, int groupId, CmdArgs args);
}