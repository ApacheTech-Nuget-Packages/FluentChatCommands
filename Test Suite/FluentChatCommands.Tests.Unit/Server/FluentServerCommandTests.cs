#if DEBUG
using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.FluentChatCommands.Exceptions;
using ApacheTech.VintageMods.FluentChatCommands.Server;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Server
{
    [TestFixture]
    public class FluentServerCommandTests
    {
        private Mock<ICoreServerAPI> _mockApi;
        private Mock<IServerPlayer> _mockPlayer;
        private readonly List<string> _registeredCommands = new();

        [SetUp]
        public void SetupMocks()
        {
            _mockPlayer = new Mock<IServerPlayer>();
            _mockApi = new Mock<ICoreServerAPI>();
            _mockApi.Setup(p =>
                p.SendMessage(It.IsAny<IServerPlayer>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EnumChatType>(), null)).Verifiable();
            _mockApi.Setup(p =>
                p.RegisterCommand(It.IsAny<ServerChatCommand>()))
                .Returns((ServerChatCommand p) =>
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
        public void FluentServerCommand_ShouldAddChildToList_WhenSubCommandAdded()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            var countBefore = concreteCommand.SubCommands.Count;
            command.HasSubCommand("sub");
            var countAfter = concreteCommand.SubCommands.Count;

            countAfter.Should().Be(countBefore + 1);
        }

        [Test]
        public void FluentServerCommand_ShouldThrowException_WhenSubCommandDuplicated()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");

            Assert.Throws<SubCommandAlreadyExistsException>(() =>
            {
                command.HasSubCommand("sub");
                command.HasSubCommand("sub");
            });
        }

        [Test]
        public void FluentServerCommand_ShouldReturnASubCommand_WhenNewSubCommandAdded()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}")
                .HasSubCommand("subCommand");

            command.Should().BeOfType<FluentServerSubCommand>();
            command.Should().BeAssignableTo<IFluentServerSubCommand>();
        }

        [Test]
        public void FluentServerCommand_ShouldSetADefaultHandler_WhenToldTo()
        {
            // ReSharper disable once ConvertToLocalFunction
            ServerChatCommandDelegate expected = (_,_, _) => { };

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}")
                .HasDefaultHandler(expected);

            var concreteCommand = (FluentServerCommand)command;

            concreteCommand.DefaultHandler
                .Should().BeSameAs(expected);

        }

        [Test]
        public void FluentServerCommand_ShouldRegisterCorrectly_WhenRegistered()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            command.RegisterWith(_mockApi.Object);

            _mockApi.Verify(api =>
                api.RegisterCommand(concreteCommand.ChatCommand),
                Times.Once);
        }

        [Test]
        public void FluentServerCommand_ShouldPopulateHelpText_WhenToldTo()
        {
            const string expected = "Hello, World!";

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}")
                .HasDescription(expected);

            var concreteCommand = (FluentServerCommand)command;

            concreteCommand.ChatCommand.Description
                .Should().BeSameAs(expected);
        }

        [Test]
        public void FluentServerCommand_ShouldThrowException_WhenOverridingPrivilege()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            command
                .Invoking(c => c
                    .RequiresPrivilege(Privilege.controlserver)
                    .RequiresPrivilege(Privilege.root))
                .Should().Throw<PrivilegeOverrideException>()
                .Where(p => p.ChatCommand == concreteCommand.ChatCommand)
                .Where(p => p.CommandName == concreteCommand.ChatCommand.Command)
                .Where(p => p.ExistingPrivilege == Privilege.controlserver)
                .Where(p => p.NewPrivilege == Privilege.root);
        }

        [Test]
        public void FluentServerCommand_ShouldThrowException_WhenPassedEmptyPrivilege()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            command
                .Invoking(c => c.RequiresPrivilege(""))
                .Should().Throw<PrivilegeMismatchException>()
                .Where(p => p.ChatCommand == concreteCommand.ChatCommand)
                .Where(p => p.CommandName == concreteCommand.ChatCommand.Command);
        }

        [Test]
        public void FluentServerCommand_DefaultHandlerShouldNotBeNull_WhenConstructed()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;
            concreteCommand.ChatCommand.handler.Should().NotBeNull();
        }

        [Test]
        public void FluentServerCommand_ShouldHandleCall_WhenCalled()
        {
            var commandName = $"{Guid.NewGuid()}";
            var command = FluentChat.ServerCommand(commandName);

            command.HasDefaultHandler((_, _, _) => { })
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentServerCommand)command;

            concreteCommand.ChatCommand.Invoking(p => p.CallHandler(
                _mockPlayer.Object, 1, new CmdArgs()))
                .Should().NotThrow();
        }

        [Test]
        public void FluentServerCommand_ShouldCallDefaultHandler_WhenNoArgumentsPassed()
        {
            var args = new CmdArgs();
            var mock = new Mock<ServerChatCommandDelegate>();
            mock.Setup(p => p(_mockPlayer.Object, 1, args)).Verifiable();

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");

            command.HasDefaultHandler(mock.Object)
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentServerCommand)command;
            concreteCommand.ChatCommand.CallHandler(_mockPlayer.Object, 1, args);

            mock.Verify(p => p(_mockPlayer.Object, 1, args), Times.Once);
        }

        [Test]
        public void FluentServerCommand_ShouldCallSubCommand_WhenValidArgumentPassed()
        {
            // ReSharper disable once ConvertToLocalFunction
            ServerChatCommandDelegate handler = (_, _, _) => { };
            var args = new CmdArgs("subCommand Test");

            var mock = new Mock<FluentServerSubCommandHandler>();
            mock.Setup(p => p(It.IsAny<string>(), _mockPlayer.Object, 1, args)).Verifiable();

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");

            command.HasDefaultHandler(handler)
                .HasSubCommand("subCommand")
                .WithHandler(mock.Object)
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentServerCommand)command;
            concreteCommand.ChatCommand.CallHandler(_mockPlayer.Object, 1, args);

            mock.Verify(p => p(It.IsAny<string>(), _mockPlayer.Object, 1, args), Times.Once);
        }

        [TestCase]
        [TestCase("anyString")]
        [TestCase("anyString", "withArgs")]
        public void FluentServerCommand_ShouldThrowException_WhenCalledIfNoHandlersExist(
            params string[] args)
        {
            var cmdArgs = new CmdArgs("Any");
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            concreteCommand.Invoking(p =>
                    p.ChatCommand.CallHandler(null, 1, cmdArgs))
                .Should().Throw<NoHandlersFoundException>();
        }

        [Test]
        public void FluentServerCommand_ShouldOutputSyntaxMessage_WhenOrphanedDefaultHandlerIsFound()
        {
            var args = new CmdArgs("");
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            command
                .HasSubCommand("subCommand")
                .WithHandler((_, _, _, _) => { })
                .RegisterWith(_mockApi.Object);

            concreteCommand.ChatCommand.CallHandler(_mockPlayer.Object, 1, args);

            _mockApi.Verify(p =>
                    p.SendMessage(_mockPlayer.Object, 1, concreteCommand.ChatCommand.GetHelpMessage(), EnumChatType.Notification, null),
                Times.Once);
        }

        [Test]
        public void FluentServerCommand_ShouldThrowException_WhenOrphanedSubCommandIsFound()
        {
            var args = new CmdArgs("subCommand");
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var concreteCommand = (FluentServerCommand)command;

            command.HasSubCommand("subCommand");
            command.RegisterWith(_mockApi.Object);

            concreteCommand.Invoking(p =>
                    p.ChatCommand.CallHandler(null, 1, args))
                .Should().Throw<OrphanedSubCommandException>()
                .Where(p => p.ChatCommand == concreteCommand.ChatCommand)
                .Where(p => p.CommandName == concreteCommand.ChatCommand.Command);
        }

        [TestCase]
        [TestCase("a")]
        [TestCase("a", "b")]
        [TestCase("a", "b", "c")]
        [TestCase("a", "b", "c", "d")]
        public void FluentServerCommand_ShouldDisplayCorrectSyntaxMessage_WhenMethodCalled(params string[] subCommands)
        {
            var commandName = $"{Guid.NewGuid()}";
            var command = FluentChat.ServerCommand(commandName);
            var concreteCommand = (FluentServerCommand)command;

            var syntaxMessage = subCommands.Length > 0 ? $"[{string.Join('|', subCommands)}]" : "";

            var helpMessage = $"{commandName}: \nSyntax: {syntaxMessage}";

            command
                .RegisterWith(_mockApi.Object);

            foreach (var subCommand in subCommands)
            {
                command.HasSubCommand(subCommand)
                    .WithHandler((_, _, _, _) => { });
            }

            concreteCommand.ChatCommand.GetHelpMessage().Should().Match(helpMessage);
        }
    }
}
#endif