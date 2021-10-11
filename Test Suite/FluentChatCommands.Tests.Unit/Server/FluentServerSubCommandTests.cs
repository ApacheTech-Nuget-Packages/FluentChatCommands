#if DEBUG
using System;
using System.Collections.Generic;
using ApacheTech.VintageMods.FluentChatCommands.Server;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable ConvertToLocalFunction

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Server
{
    [TestFixture]
    public class FluentServerSubCommandTests
    {
        private Mock<ICoreServerAPI> _mockApi;
        private readonly List<string> _registeredCommands = new();

        [SetUp]
        public void SetupMocks()
        {
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
        public void FluentServerSubCommand_ShouldReturnParentCommand_WhenHandlerAdded()
        {
            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}");
            var subCommand = command.HasSubCommand("subCommand");
            var parent = subCommand.WithHandler((_, _, _, _) => { });

            Assert.IsInstanceOf<IFluentServerCommand>(command);
            Assert.IsAssignableFrom<FluentServerCommand>(command);

            Assert.IsInstanceOf<IFluentServerSubCommand>(subCommand);
            Assert.IsAssignableFrom<FluentServerSubCommand>(subCommand);

            Assert.AreSame(command, parent);
            Assert.IsInstanceOf<IFluentServerCommand>(parent);
            Assert.IsAssignableFrom<FluentServerCommand>(parent);
        }

        [Test]
        public void FluentServerSubCommand_ShouldSetHandler_WhenHandlerAdded()
        {
            FluentChatServerSubCommandHandler expected = (_, _, _, _) => { };

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}")
                .HasSubCommand("subCommand")
                .WithHandler(expected);

            var concreteCommand = (FluentServerCommand)command;

            concreteCommand.SubCommands["subCommand"].Handler
                .Should().BeSameAs(expected);
        }

        [TestCase("subCommandName")]
        [TestCase("subCommandName2 someOtherArg")]
        [TestCase("subCommandName2 someOtherArg andAnotherArg")]
        public void FluentServerSubCommand_ShouldHaveAccessToCommandName_WhenCalled(string args)
        {
            var cmdArgs = new CmdArgs(args);
            var subCommandName = cmdArgs.PeekWord();

            FluentChatServerSubCommandHandler handler = TestHandler;

            var command = FluentChat.ServerCommand($"{Guid.NewGuid()}")
                .HasSubCommand(subCommandName).WithHandler(handler)
                .RegisterWith(_mockApi.Object);

            var concreteCommand = (FluentServerCommand)command;
            concreteCommand.CallHandler(It.IsAny<IServerPlayer>(), 1, cmdArgs.Clone());

            void TestHandler(string subCommandNameInHandler, IServerPlayer player, int groupIdInHandler, CmdArgs argsInHandler)
            {
                subCommandNameInHandler.Should().BeSameAs(subCommandName);
                groupIdInHandler.Should().Be(1);
                argsInHandler.Length.Should().Be(Math.Max(0, cmdArgs.Length - 1));
                argsInHandler.PopWord().Should().NotBe(subCommandNameInHandler);
            }
        }
    }
}
#endif