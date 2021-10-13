#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ApacheTech.VintageMods.FluentChatCommands.Client;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable ConvertToLocalFunction

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Client
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class FluentClientSubCommandTests
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
        public void FluentClientSubCommand_ShouldReturnParentCommand_WhenHandlerAdded()
        {
            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}");
            var subCommand = command.HasSubCommand("subCommand");
            var parent = subCommand.WithHandler((_, _, _) => { });

            Assert.IsInstanceOf<IFluentClientCommand>(command);
            Assert.IsAssignableFrom<FluentClientCommand>(command);

            Assert.IsInstanceOf<IFluentClientSubCommand>(subCommand);
            Assert.IsAssignableFrom<FluentClientSubCommand>(subCommand);

            Assert.AreSame(command, parent);
            Assert.IsInstanceOf<IFluentClientCommand>(parent);
            Assert.IsAssignableFrom<FluentClientCommand>(parent);
        }

        [Test]
        public void FluentClientSubCommand_ShouldSetHandler_WhenHandlerAdded()
        {
            FluentClientSubCommandHandler expected = (_, _, _) => { };

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}")
                .HasSubCommand("subCommand")
                .WithHandler(expected);

            var concreteCommand = (FluentClientCommand)command;

            concreteCommand.SubCommands["subCommand"].Handler
                .Should().BeSameAs(expected);
        }

        [TestCase("subCommandName")]
        [TestCase("subCommandName2 someOtherArg")]
        [TestCase("subCommandName2 someOtherArg andAnotherArg")]
        public void FluentClientSubCommand_ShouldHaveAccessToCommandName_WhenCalled(string args)
        {
            var cmdArgs = new CmdArgs(args);
            var subCommandName = cmdArgs.PeekWord();

            FluentClientSubCommandHandler handler = TestHandler;

            var command = FluentChat.ClientCommand($"{Guid.NewGuid()}")
                .HasSubCommand(subCommandName).WithHandler(handler)
                .RegisterWith(_mockApi.Object);
            
            var concreteCommand = (FluentClientCommand)command;
            concreteCommand.CallHandler(1, cmdArgs.Clone());

            void TestHandler(string subCommandNameInHandler, int groupIdInHandler, CmdArgs argsInHandler)
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