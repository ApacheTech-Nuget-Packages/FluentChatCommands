using ApacheTech.VintageMods.FluentChatCommands.Exceptions;

namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    internal sealed class FluentServerSubCommand : IFluentServerSubCommand
    {
        private readonly IFluentServerCommand _parent;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="FluentServerSubCommand"/> class.
        /// </summary>
        /// <param name="parentServerCommand">The parent server command.</param>
        internal FluentServerSubCommand(IFluentServerCommand parentServerCommand)
        {
            _parent = parentServerCommand;
        }

        internal FluentServerSubCommandHandler Handler { get; private set; }

        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A server-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        public IFluentServerCommand WithHandler(FluentServerSubCommandHandler handler)
        {
            Handler = handler;
            return _parent;
        }
    }
}