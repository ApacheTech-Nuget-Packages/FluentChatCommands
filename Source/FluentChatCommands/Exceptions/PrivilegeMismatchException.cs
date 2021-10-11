using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     This exception occurs when this privilege requirement passed to the command is not found within the game API.
    /// </summary>
    /// <seealso cref="FluentChatException" />
    [Serializable]
    public sealed class PrivilegeMismatchException : FluentChatException
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="PrivilegeMismatchException"/> class.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <param name="message">The message to show to the user.</param>
        internal PrivilegeMismatchException(ChatCommand command, string message) : base(command, message)
        {
        }
    }
}