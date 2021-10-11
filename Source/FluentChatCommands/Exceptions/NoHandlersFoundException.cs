using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     This exception occurs when no handlers or sub-command handlers have for a given command, and arguments are passed to the command for parsing.
    /// </summary>
    /// <seealso cref="FluentChatException" />
    [Serializable]
    public sealed class NoHandlersFoundException : FluentChatException
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="NoHandlersFoundException"/> class.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <param name="message">The message to show to the user.</param>
        internal NoHandlersFoundException(ChatCommand command, string message) : base(command, message)
        {
        }
    }
}