using ApacheTech.VintageMods.FluentChatCommands.Exceptions;

// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Server
{
    /// <summary>
    ///     Represents a sub-command, for a server-side chat command, built with the FluentChat builder.
    /// </summary>
    public interface IFluentServerSubCommand
    {
        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A server-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        IFluentServerCommand WithHandler(FluentChatServerSubCommandHandler handler);
    }
}