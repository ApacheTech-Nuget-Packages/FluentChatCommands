using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     This exception occurs when a sub-command of the same name has already been declared for a given command, when attempting to add a replacement.
    /// </summary>
    /// <seealso cref="FluentChatException" />
    [Serializable]
    public sealed class SubCommandAlreadyExistsException : FluentChatException
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="SubCommandAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <param name="message">The message to show to the user.</param>
        internal SubCommandAlreadyExistsException(ChatCommand command, string message) : base(command, message)
        {
        }
    }
}