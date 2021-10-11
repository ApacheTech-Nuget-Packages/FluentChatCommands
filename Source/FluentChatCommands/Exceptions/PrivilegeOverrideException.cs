using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Exceptions
{
    /// <summary>
    ///     This exception occurs when a privilege requirement has already been declared for a given command, when attempting to assign a new one.
    /// </summary>
    /// <seealso cref="FluentChatException" />
    [Serializable]
    public sealed class PrivilegeOverrideException : FluentChatException
    {
        /// <summary>
        ///     The existing privilege, currently set on the command.
        /// </summary>
        /// <value>The existing privilege.</value>
        public string ExistingPrivilege { get; }

        /// <summary>
        ///     The new privilege requirement that requested to be set.
        /// </summary>
        /// <value>The new privilege.</value>
        public string NewPrivilege { get; }

        /// <summary>
        ///     Initialises a new instance of the <see cref="PrivilegeOverrideException"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="message">The message.</param>
        /// <param name="privilege">The privilege.</param>
        internal PrivilegeOverrideException(ChatCommand command, string message, string privilege) : base(command, message)
        {
            ExistingPrivilege = ChatCommand.RequiredPrivilege;
            NewPrivilege = privilege;
        }
    }
}