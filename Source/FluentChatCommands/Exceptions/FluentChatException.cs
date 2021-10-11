using System;
using Vintagestory.API.Common;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     Acts as a base for all exceptions within the FluentChat package.
    ///     Implements the <see cref="Exception" /> base class.
    /// </summary>
    /// <seealso cref="Exception" />
    public abstract class FluentChatException : Exception
    {
        /// <summary>
        ///     The name of the command that caused the exception.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        ///     The <see cref="ChatCommand"/> object that holds further information about this command.
        /// </summary>
        public ChatCommand ChatCommand { get; }
        
        /// <summary>
        ///     Initialises a new instance of the <see cref="FluentChatException"/> class.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <param name="message">The message to show to the user.</param>
        protected FluentChatException(ChatCommand command, string message) : base(message)
        {
            ChatCommand = command;
            CommandName = command.Command;
        }
    }
}