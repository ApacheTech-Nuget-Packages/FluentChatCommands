using System;
using System.Collections.Generic;
using System.Linq;
using ApacheTech.VintageMods.FluentChatCommands.Contracts;
using Vintagestory.API.Common;

// ReSharper disable MemberCanBeProtected.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Abstractions
{
    internal abstract class FluentChatCommandBase : IHaveSubCommands, IHandleChatCommands
    {
        protected ICoreAPI UApi { get; }

        internal EnumAppSide Side => UApi.Side;

        protected internal FluentChatCommandHandler CallHandler { get; protected set; }

        Dictionary<string, IHandleChatCommands> IHaveSubCommands.SubCommands { get; } = new(StringComparer.OrdinalIgnoreCase);

        protected FluentChatCommandBase(ICoreAPI api)
        {
            UApi = api;
            CallHandler = DefaultCallHandler;
        }

        internal virtual void DefaultCallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            // Do nothing, by default.
        }
        
        public void Handle(IPlayer player, int groupId, CmdArgs args)
        {
            var subCommand = args.PeekWord("").ToLowerInvariant();
            if ((this as IHaveSubCommands).SubCommands.ContainsKey(subCommand))
            {
                subCommand = args.PopWord().ToLowerInvariant();
                (this as IHaveSubCommands).SubCommands[subCommand].Handle(player, groupId, args);
                return;
            }
            CallHandler(player, groupId, args);
        }

        protected string GetSyntax()
        {
            var subCommands = (this as IHaveSubCommands).SubCommands;
            return subCommands.Count > 0
                ? $"[{string.Join<string>("|", subCommands.Keys.ToArray())}]"
                : "";
        }
    }
}