using System.Collections.Generic;

namespace ApacheTech.VintageMods.FluentChatCommands.Contracts
{
    public interface IHaveSubCommands
    {
        internal Dictionary<string, IHandleChatCommands> SubCommands { get; }
    }
}