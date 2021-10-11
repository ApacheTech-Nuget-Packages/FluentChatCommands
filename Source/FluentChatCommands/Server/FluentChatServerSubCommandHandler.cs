using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    // TODO: XML Doc needed: FluentChatServerSubCommandHandler
    public delegate void FluentChatServerSubCommandHandler(string subCommandName, IServerPlayer player, int groupId, CmdArgs args);
}