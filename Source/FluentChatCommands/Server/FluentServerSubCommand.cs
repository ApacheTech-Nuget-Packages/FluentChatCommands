using ApacheTech.VintageMods.FluentChatCommands.Exceptions;

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

        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A server-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        public IFluentServerCommand WithHandler(FluentChatServerSubCommandHandler handler)
        {
            Handler = handler;
            return _parent;
        }
    }
}