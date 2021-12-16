using ApacheTech.VintageMods.FluentChatCommands.Exceptions;

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    internal sealed class FluentClientSubCommand : IFluentClientSubCommand
    {
        private readonly IFluentClientCommand _parent;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="FluentClientSubCommand"/> class.
        /// </summary>
        /// <param name="parentClientCommand">The parent client command.</param>
        internal FluentClientSubCommand(IFluentClientCommand parentClientCommand)
        {
            _parent = parentClientCommand;
        }

        internal FluentClientSubCommandHandler Handler { get; private set; }

        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A client-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        public IFluentClientCommand WithHandler(FluentClientSubCommandHandler handler)
        {
            Handler = handler;
            return _parent;
        }
    }
}