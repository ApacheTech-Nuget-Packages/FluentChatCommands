using ApacheTech.VintageMods.FluentChatCommands.Abstractions;
using ApacheTech.VintageMods.FluentChatCommands.Contracts;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.FluentChatCommands
{
    /// <summary>
    ///     Represents a sub-command for a FluentChat command.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent this sub-command will be added to.</typeparam>
    public interface IFluentChatSubCommandBuilder<TParent> : IHaveSubCommands
    {
        /// <summary>
        ///     Adds an alias for this sub-command; another way for the user to reach the same handler.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatSubCommandBuilder<TParent> WithAlias(string alias);

        /// <summary>
        ///     Specifies the default command handler to use for this command.
        ///     If no default handler is set, sub-commands will still function, and calling the command with no arguments will show
        ///     the command's help message to the user.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatSubCommandBuilder<TParent> WithHandler(FluentChatCommandHandler handler);

        /// <summary>
        ///     Adds a sub-command, with it's own command handler.
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="builder">A factory, used to build the sub command.</param>
        /// <returns>Returns the same instance of the command, for further composition, if needed.</returns>
        IFluentChatSubCommandBuilder<TParent> HasSubCommand(string subCommandName,
            FluentChatSubCommandFactory<IFluentChatSubCommandBuilder<TParent>> builder);

        /// <summary>
        ///     Builds this sub-command, adding it to the parent's sub-commands list, along with any aliases that may have been specified.
        /// </summary>
        /// <returns>Returns back to the parent of the sub-command, for further composition, if needed.</returns>
        TParent Build();
    }
}