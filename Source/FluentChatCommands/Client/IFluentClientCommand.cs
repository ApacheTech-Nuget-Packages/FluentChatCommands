using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Client
{
    /// <summary>
    ///     Represents a client-side chat command, built with the FluentChat builder.
    /// </summary>
    public interface IFluentClientCommand
    {
        /// <summary>
        ///     Specifies the description for the command.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        IFluentClientCommand HasDescription(string description);

        /// <summary>
        ///     Specifies the default command handler to use for this command.
        ///     If no default handler is set, sub-commands will still function, and calling the command with no arguments will show
        ///     the command's help message to the user.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        IFluentClientCommand HasDefaultHandler(ClientChatCommandDelegate handler);

        /// <summary>
        ///     Registers this command with the client-side API.
        ///     FluentChat supports lazy loading, so this method can be called at any time along the build process.
        /// </summary>
        /// <param name="capi">The client-side API to register the command with.</param>
        IFluentClientCommand RegisterWith(ICoreClientAPI capi);

        /// <summary>
        ///     Adds a sub-command, with it's own command handler.
        ///     All sub-commands must have handlers set, or this command will throw a <see cref="OrphanedSubCommandException" />
        ///     exception when called.
        /// </summary>
        /// <param name="subCommandName">Name of the sub command.</param>
        /// <returns>Returns the sub-command, so that a handler can be set.</returns>
        IFluentClientSubCommand HasSubCommand(string subCommandName);
    }
}