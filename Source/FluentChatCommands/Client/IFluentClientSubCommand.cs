using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using Vintagestory.API.Common;

// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    /// <summary>
    ///     Represents a sub-command, for a client-side chat command, built with the FluentChat builder.
    /// </summary>
    public interface IFluentClientSubCommand
    {
        /// <summary>
        ///     Specifies the command handler to use, when the user calls the sub-command.
        ///     All sub-commands must have handlers set, or the parent command will throw a
        ///     <see cref="OrphanedSubCommandException" /> exception when called.
        /// </summary>
        /// <param name="handler">A client-side command handler.</param>
        /// <returns>Returns the parent command.</returns>
        IFluentClientCommand WithHandler(FluentClientSubCommandHandler handler);
    }
}