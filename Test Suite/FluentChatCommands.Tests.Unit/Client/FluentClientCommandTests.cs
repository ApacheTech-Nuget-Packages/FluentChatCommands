#if DEBUG
using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.FluentChatCommands.Client;
using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Client
{
    
    [TestFixture]
    public class FluentClientCommandTests
    {
        private Mock<ICoreClientAPI> _mockApi;
        private readonly List<string> _registeredCommands = new();

        [SetUp]
        public void SetupMocks()
        {
            _mockApi = new Mock<ICoreClientAPI>();
            _mockApi.Setup(p =>
                p.ShowChatMessage(It.IsAny<string>())).Verifiable();
            _mockApi.Setup(p =>
                p.RegisterCommand(It.IsAny<ClientChatCommand>()))
                .Returns((ClientChatCommand p) =>
                {
                    if (_registeredCommands.Contains(p.Command))
                    {
                        return false;
                    }
                    _registeredCommands.Add(p.Command);
                    return true;
                });

        }

        [Test]
        public void FluentClientCommand_ShouldAddChildToList_WhenSubCommandAdded()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            var countBefore = concreteCommand.SubCommands.Count;
            command.HasSubCommand("sub");
            var countAfter = concreteCommand.SubCommands.Count;

            countAfter.Should().Be(countBefore + 1);
        }

        [Test]
        public void FluentClientCommand_ShouldThrowException_WhenSubCommandDuplicated()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            command.Invoking(p =>
            {
                p.HasSubCommand("sub");
                p.HasSubCommand("sub");
            }).Should().Throw<SubCommandAlreadyExistsException>();
            
        }

        [Test]
        public void FluentClientCommand_ShouldReturnASubCommand_WhenNewSubCommandAdded()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}")
                .HasSubCommand("subCommand");

            command.Should().BeOfType<FluentClientSubCommand>();
            command.Should().BeAssignableTo<IFluentClientSubCommand>();
        }

        [Test]
        public void FluentClientCommand_ShouldSetADefaultHandler_WhenToldTo()
        {
            // ReSharper disable once ConvertToLocalFunction
            ClientChatCommandDelegate expected = (_, _) => { };

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}")
                .HasDefaultHandler(expected);

            var concreteCommand = (FluentClientCommand)command;

            concreteCommand.DefaultHandler
                .Should().BeSameAs(expected);

        }

        [Test]
        public void FluentClientCommand_ShouldRegisterCorrectly_WhenRegistered()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            command.RegisterWith(_mockApi.Object);

            _mockApi.Verify(api =>
                api.RegisterCommand(concreteCommand.ChatCommand),
                Times.Once);
        }

        [Test]
        public void FluentClientCommand_ShouldPopulateHelpText_WhenToldTo()
        {
            const string expected = "Hello, World!";

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}")
                .HasDescription(expected);

            var concreteCommand = (FluentClientCommand)command;

            concreteCommand.ChatCommand.Description
                .Should().BeSameAs(expected);
        }
        
        [Test]
        public void FluentClientCommand_DefaultHandlerShouldNotBeNull_WhenConstructed()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;
            concreteCommand.ChatCommand.handler.Should().NotBeNull();
        }

        [Test]
        public void FluentClientCommand_ShouldHandleCall_WhenCalled()
        {
            var mockPlayer = new Mock<IPlayer>();

            var commandName = $"{Guid.NewGuid()}";
            var command = FluentChat.ClientCommand(commandName);

            command.HasDefaultHandler((_, _) => { })
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentClientCommand)command;

            concreteCommand.ChatCommand.Invoking(p => p.CallHandler(
                mockPlayer.Object, 1, new CmdArgs()))
                .Should().NotThrow();
        }

        [Test]
        public void FluentClientCommand_ShouldCallDefaultHandler_WhenNoArgumentsPassed()
        {
            var args = new CmdArgs();
            var mock = new Mock<ClientChatCommandDelegate>();
            mock.Setup(p => p(1, args)).Verifiable();

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");

            command.HasDefaultHandler(mock.Object)
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentClientCommand)command;
            concreteCommand.ChatCommand.CallHandler(null, 1, args);

            mock.Verify(p => p(1, args), Times.Once);
        }

        [Test]
        public void FluentClientCommand_ShouldCallSubCommand_WhenValidArgumentPassed()
        {
            // ReSharper disable once ConvertToLocalFunction
            ClientChatCommandDelegate handler = (_, _) => { };
            var args = new CmdArgs("subCommand Test");

            var mock = new Mock<FluentClientSubCommandHandler>();
            mock.Setup(p => p(It.IsAny<string>(), 1, args)).Verifiable();

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");

            command.HasDefaultHandler(handler)
                .HasSubCommand("subCommand")
                .WithHandler(mock.Object)
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentClientCommand)command;
            concreteCommand.ChatCommand.CallHandler(null, 1, args);

            mock.Verify(p => p(It.IsAny<string>(), 1, It.IsAny<CmdArgs>()), Times.Once);
        }

        [TestCase]
        [TestCase("anyString")]
        [TestCase("anyString", "withArgs")]
        public void FluentClientCommand_ShouldThrowException_WhenCalledIfNoHandlersExist(
            params string[] args)
        {
            var cmdArgs = new CmdArgs("Any");
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            concreteCommand.Invoking(p =>
                    p.ChatCommand.CallHandler(null, 1, cmdArgs))
                .Should().Throw<NoHandlersFoundException>();
        }

        [Test]
        public void FluentClientCommand_ShouldOutputSyntaxMessage_WhenOrphanedDefaultHandlerIsFound()
        {
            var args = new CmdArgs("");
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            command
                .HasSubCommand("subCommand")
                .WithHandler((_, _, _) => { })
                .RegisterWith(_mockApi.Object);

            concreteCommand.ChatCommand.CallHandler(null, 1, args);

            _mockApi.Verify(p =>
                p.ShowChatMessage(concreteCommand.ChatCommand.GetHelpMessage()), Times.Once);
        }

        [Test]
        public void FluentClientCommand_ShouldThrowException_WhenOrphanedSubCommandIsFound()
        {
            var args = new CmdArgs("subCommand");
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            command.HasSubCommand("subCommand");
            command.RegisterWith(_mockApi.Object);

            concreteCommand.Invoking(p =>
                    p.ChatCommand.CallHandler(null, 1, args))
                .Should().Throw<OrphanedSubCommandException>();
        }

        [TestCase]
        [TestCase("a")]
        [TestCase("a", "b")]
        [TestCase("a", "b", "c")]
        [TestCase("a", "b", "c", "d")]
        public void FluentClientCommand_ShouldDisplayCorrectSyntaxMessage_WhenMethodCalled(params string[] subCommands)
        {
            var commandName = $"{Guid.NewGuid()}";
            var command = FluentChat.ClientCommand(commandName);
            var concreteCommand = (FluentClientCommand)command;

            var syntaxMessage = subCommands.Length > 0 ? $"[{string.Join('|', subCommands)}]" : "";

            var helpMessage = $"{commandName}: \nSyntax: {syntaxMessage}";

            command
                .RegisterWith(_mockApi.Object);

            foreach (var subCommand in subCommands)
            {
                command.HasSubCommand(subCommand)
                    .WithHandler((_, _, _) => { });
            }

            concreteCommand.ChatCommand.GetHelpMessage().Should().Match(helpMessage);
        }
        
        [TestCase("invalidSubCommand")]
        [TestCase("invalidSubCommand withArguments")]
        public void FluentClientCommand_ShouldOutputSyntaxMessage_WhenInvalidSubCommandPassed(string arguments)
        {
            var args = new CmdArgs(arguments);
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentClientCommand)command;

            command
                .HasDefaultHandler((_, _) => { })
                .HasSubCommand("subCommand").WithHandler((_, _, _) => { })
                .RegisterWith(_mockApi.Object);

            concreteCommand.ChatCommand.CallHandler(null, 1, args);

            _mockApi.Verify(p =>
                p.ShowChatMessage(concreteCommand.ChatCommand.GetHelpMessage()), Times.Once);
        }
    }
}
#endif