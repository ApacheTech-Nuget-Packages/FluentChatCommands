namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    internal class FluentServerSubCommand : IFluentServerSubCommand
    {
        private readonly IFluentServerCommand _parent;

        internal FluentServerSubCommand(IFluentServerCommand parentServerCommand)
        {
            _parent = parentServerCommand;
        }

        internal FluentChatServerSubCommandHandler Handler { get; private set; }

        public IFluentServerCommand WithHandler(FluentChatServerSubCommandHandler handler)
        {
            Handler = handler;
            return _parent;
        }
    }
}