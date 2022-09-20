using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Contracts
{
    public interface IHandleChatCommands
    {
        void Handle(IPlayer player, int groupId, CmdArgs args);
    }
}