#if DEBUG
using System;
using ApacheTech.VintageMods.FluentChatCommands.Server;
using FluentAssertions;
using NUnit.Framework;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Server
{
    [TestFixture]
    public class FluentChatServerTests
    {
        [Test]
        public void FluentChat_ShouldReturnSingleton_WhenCalledFromServer()
        {
            var commandName = $"{Guid.NewGuid()}";
            var currentNumberOfCommands = FluentChat.ServerCommandCount;

            var command = FluentChat.ServerCommand(commandName);
            var commandDuplicate = FluentChat.ServerCommand(commandName);

            commandDuplicate.Should().BeSameAs(command);
            FluentChat.ServerCommandCount.Should().Be(currentNumberOfCommands + 1);
        }

        [Test]
        public void FluentChat_ShouldReturnAServerCommand_WhenCalledFromServer()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            command.Should().BeOfType<FluentServerCommand>();
            command.Should().BeAssignableTo<IFluentServerCommand>();
        }
        
        [TestCase("")]
        [TestCase(null)]
        public void FluentChat_ShouldThrowException_WhenPassedEmptyStringFromServer(string invalidCommandName)
        {
            var mockCall = new Action<string>(s => FluentChat.ServerCommand(s));
            mockCall.Invoking(p => p(invalidCommandName)).Should().Throw<ArgumentException>();
        }
    }
}
#endif