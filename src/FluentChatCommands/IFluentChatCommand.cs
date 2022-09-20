using ApacheTech.VintageMods.FluentChatCommands.Abstractions;
using ApacheTech.VintageMods.FluentChatCommands.Contracts;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands
{
    /// <summary>
    ///     Represents a chat command, built with the FluentChat builder.
    /// </summary>
    public interface IFluentChatCommand : IHaveSubCommands
    {
        /// <summary>
        ///     Specifies the description for the command.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatCommand WithDescription(string description);

        /// <summary>
        ///     Specifies the required privilege a user must have, before the can use this command.
        /// </summary>
        /// <param name="privilege">The privilege to set.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed..</returns>
        IFluentChatCommand RequiresPrivilege(string privilege);

        /// <summary>
        ///     Specifies the default command handler to use for this command.
        ///     If no default handler is set, sub-commands will still function, and calling the command with no arguments will show
        ///     the command's help message to the user.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatCommand WithHandler(FluentChatCommandHandler handler);

        /// <summary>
        ///     Adds a sub-command, with it's own command handler.
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="builder">A factory, used to build the sub command.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatCommand HasSubCommand(string subCommandName, FluentChatSubCommandFactory<IFluentChatCommand> builder);
    }
}