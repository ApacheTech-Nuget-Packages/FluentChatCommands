using System.Collections.Generic;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.FluentChatCommands.Abstractions;
using ApacheTech.VintageMods.FluentChatCommands.Contracts;
using Vintagestory.API.Common;

// ReSharper disable MemberCanBePrivate.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Implementations
{
#if UNIT_TEST
    internal class FluentChatSubCommand<TParent> : FluentChatCommandBase, IFluentChatSubCommandBuilder<TParent> where TParent : IHaveSubCommands
#else

    internal sealed class FluentChatSubCommand<TParent> : FluentChatCommandBase, IFluentChatSubCommandBuilder<TParent> where TParent : IHaveSubCommands
#endif
    {
        internal readonly TParent Parent;
        internal readonly List<string> Aliases = new();

        public FluentChatSubCommand(string subCommandName, TParent parent, ICoreAPI api) : base(api)
        {
            Parent = parent;
            Aliases.Add(subCommandName);
        }

        public IFluentChatSubCommandBuilder<TParent> WithAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return this;
            Aliases.AddIfNotPresent(alias.ToLowerInvariant());
            return this;
        }

        public IFluentChatSubCommandBuilder<TParent> WithHandler(FluentChatCommandHandler handler)
        {
            if (!CallHandler.Method.Name.Equals(nameof(DefaultCallHandler))) return this;
            CallHandler = handler;
            return this;
        }

        public IFluentChatSubCommandBuilder<TParent> HasSubCommand(string subCommandName, FluentChatSubCommandFactory<IFluentChatSubCommandBuilder<TParent>> builder)
        {
            var subCommand = new FluentChatSubCommand<IFluentChatSubCommandBuilder<TParent>>(subCommandName, this, UApi);
            return builder(subCommand);
        }

        public TParent Build()
        {
            foreach (var alias in Aliases)
            {
                Parent.SubCommands.AddIfNotPresent(alias, this);
            }
            return Parent;
        }
    }
}