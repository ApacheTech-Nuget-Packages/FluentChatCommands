using ApacheTech.VintageMods.FluentChatCommands.Exceptions;

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    internal class FluentClientSubCommand : IFluentClientSubCommand
    {
        private readonly IFluentClientCommand _parent;

        internal FluentClientSubCommand(IFluentClientCommand parentClientCommand)
        {
            _parent = parentClientCommand;
        }

        internal FluentChatClientSubCommandHandler Handler { get; private set; }

        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A client-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        public IFluentClientCommand WithHandler(FluentChatClientSubCommandHandler handler)
        {
            Handler = handler;
            return _parent;
        }
    }
}