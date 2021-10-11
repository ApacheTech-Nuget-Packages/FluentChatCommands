using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     This exception occurs when a command is called, if one or more of its sub-commands have not had their handlers set.
    /// </summary>
    /// <seealso cref="FluentChatException" />
    /// <remarks>
    ///     This is primarily a safety concern, so that no command can be shipped to production without being in full working
    ///     condition. If you feel that this is too cautious, and it would be better to only throw this exception when the
    ///     offending sub-command is called, please submit an Feedback Report or User Story on the Issue Tracker for this NuGet
    ///     Package, and it might be included in a later release, as an opt-in.
    /// </remarks>
    [Serializable]
    public sealed class OrphanedSubCommandException : FluentChatException
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="OrphanedSubCommandException" /> class.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <param name="message">The message to show to the user.</param>
        internal OrphanedSubCommandException(ChatCommand command, string message) : base(command, message)
        {
        }
    }
}