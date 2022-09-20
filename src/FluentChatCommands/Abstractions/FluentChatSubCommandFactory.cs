namespace ApacheTech.VintageMods.FluentChatCommands.Abstractions
{
    /// <summary>
    ///     A factory, used to build sub-commands for FluentChat commands.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent command.</typeparam>
    /// <param name="builder">The sub-command fluent builder, used to provide implementation to the newly created sub-command.</param>
    /// <returns></returns>
    public delegate TParent FluentChatSubCommandFactory<TParent>(IFluentChatSubCommandBuilder<TParent> builder);
}