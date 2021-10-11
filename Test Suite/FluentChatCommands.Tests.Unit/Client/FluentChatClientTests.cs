#if DEBUG
using System;
using ApacheTech.VintageMods.FluentChatCommands.Client;
using FluentAssertions;
using NUnit.Framework;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Client
{
    [TestFixture]
    public class FluentChatClientTests
    {
        [Test]
        public void FluentChat_ShouldReturnSingleton_WhenCalledFromClient()
        {
            var commandName = $"{Guid.NewGuid()}";
            var currentNumberOfCommands = FluentChat.ClientCommandCount;

            var command = FluentChat.ClientCommand(commandName);
            var commandDuplicate = FluentChat.ClientCommand(commandName);

            commandDuplicate.Should().BeSameAs(command);
            FluentChat.ClientCommandCount.Should().Be(currentNumberOfCommands + 1);
        }

        [Test]
        public void FluentChat_ShouldReturnAClientCommand_WhenCalledFromClient()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            command.Should().BeOfType<FluentClientCommand>();
            command.Should().BeAssignableTo<IFluentClientCommand>();
        }

        [TestCase("")]
        [TestCase(null)]
        public void FluentChat_ShouldThrowException_WhenPassedEmptyStringFromClient(string invalidCommandName)
        {
            var mockCall = new Action<string>(s => FluentChat.ClientCommand(s));
            mockCall.Invoking(p => p(invalidCommandName)).Should().Throw<ArgumentException>();
        }
    }
}
#endif