#if UNIT_TEST
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.FluentChatCommands.Contracts;
using ApacheTech.VintageMods.FluentChatCommands.Implementations;
using ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Mocks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class FluentChatCommandTests
    {
        private MockClientApi _mockClientApi;

        private Mock<FluentChatSubCommand<FluentChatCommand>> _sutSubCommand;

        private Mock<FluentChatCommand> _sut;

        private static string RandomValidCommandName => Guid.NewGuid().ToString();

        private const string CommandName = "test";
        private const string SubCommandName = "testSubCommand";

        #region Initialisation       

        [SetUp]
        public void SetupMocks()
        {
            _mockClientApi = new MockClientApi();
            var command = FluentChat
                .RegisterCommand(CommandName, _mockClientApi.Api)
                .As<FluentChatCommand>().Command
                .As<ClientChatCommand>().With(p => p.handler = null);

            _sut = new Mock<FluentChatCommand>(command, _mockClientApi.Api) { CallBase = true };
            _sut.Setup(p => p.DefaultCallHandler(It.IsAny<IPlayer>(), It.IsAny<int>(), It.IsAny<CmdArgs>()))
                .Verifiable();

            _sutSubCommand = new Mock<FluentChatSubCommand<FluentChatCommand>>(
                SubCommandName, _sut.Object, _mockClientApi.Api)
            { CallBase = true };

            _sutSubCommand.Setup(p => p.DefaultCallHandler(It.IsAny<IPlayer>(), It.IsAny<int>(), It.IsAny<CmdArgs>()))
                .Verifiable();
        }

        [TearDown]
        public void Teardown()
        {
            FluentChat.ClearCommands(_mockClientApi.Api);
        }

        private CmdArgs TriggerCommand(string arguments)
        {
            var args = new CmdArgs(arguments);
            _sut.Object.Command.CallHandler(It.IsAny<IPlayer>(), It.IsAny<int>(), args);
            return args;
        }

        private CmdArgs TriggerCommand()
        {
            return TriggerCommand(string.Empty);
        }

        #endregion

        #region Boilerplate

        [Test]
        public void Method_Should_When()
        {
            Assert.Pass();
        }

        #endregion

        #region FluentChatCommand: [Good Path] Proper Command Construction

        [Test]
        public void FluentChatCommand_ShouldHaveEmptyDescription_WhenFirstCreated()
        {
            _sut.Object.Command.Description.Should().BeEmpty();
        }

        [Test]
        public void FluentChatCommand_ShouldHaveNoSubCommands_WhenFirstCreated()
        {
            _sut.Object.As<IHaveSubCommands>().SubCommands.Should().BeEmpty();
        }

        [Test]
        public void FluentChatCommand_ShouldHaveDefaultCallHandler_WhenFirstCreated()
        {
            _sut.Object.CallHandler.Method.Name
                .Should().Be(nameof(_sut.Object.DefaultCallHandler));
        }

        #endregion

        #region WithHandler: [Bad Path] Invalid Parameters

        [Test]
        public void WithHandler_ShouldNotSetDefaultHandler_WhenPassedNull()
        {
            var command = FluentChat.RegisterCommand(
                RandomValidCommandName, _mockClientApi!.Api)!;

            var concreteCommand = (FluentChatCommand)command!;

            command.WithHandler(null!);

            concreteCommand.CallHandler
                .Should().BeNull();
        }

        #endregion

        #region WithHandler: [Good Path] Setting Default Handler

        private static void ThisIsTheNewHandler(IPlayer player, int i, CmdArgs cmdArgs)
        {
            // Do Stuff
        }

        [Test]
        public void WithHandler_ShouldSetNewHandler_WhenPassedValidHandler()
        {
            var fieldInfo = GetType().GetMethod(nameof(ThisIsTheNewHandler), BindingFlags.NonPublic | BindingFlags.Static)!;

            _sut.Object.WithHandler(ThisIsTheNewHandler);

            _sut.Object.CallHandler.Method.Name.Should().Be(fieldInfo.Name);
        }

        [Test]
        public void WithHandler_ShouldNotSetDefaultHandler_WhenPassedValidHandlerTwice()
        {
            var fieldInfo = GetType().GetMethod(nameof(ThisIsTheNewHandler), BindingFlags.NonPublic | BindingFlags.Static)!;

            _sut.Object.WithHandler(ThisIsTheNewHandler);
            _sut.Object.WithHandler((_, _, _) => { });

            _sut.Object.CallHandler.Method.Name.Should().Be(fieldInfo.Name);
        }

        #endregion

        #region WithDescription: [Good Path] Setting Desctiption

        [TestCase("")]
        [TestCase("Hello World!")]
        public void WithDescription_ShouldSetDescription_WhenCalled(string description)
        {
            var command = FluentChat.RegisterCommand(
                RandomValidCommandName, _mockClientApi!.Api)!;

            var concreteCommand = (FluentChatCommand)command!;

            command.WithDescription(description);

            concreteCommand.Command.Description.Should().Be(description);
        }

        [Test]
        public void WithDescription_ShouldOverwriteDescription_WhenCalledTwice()
        {
            var command = FluentChat.RegisterCommand(
                RandomValidCommandName, _mockClientApi!.Api)!;

            var concreteCommand = (FluentChatCommand)command!;

            const string initialDescription = "I will be overwritten";
            const string expectedDescription = "Hello World!";

            command.WithDescription(initialDescription);

            concreteCommand.Command.Description.Should().Be(initialDescription);

            command.WithDescription(expectedDescription);

            concreteCommand.Command.Description.Should().Be(expectedDescription);
        }

        #endregion

        #region DefaultCallHandler: [Good Path] Called As Fallback Handler

        [TestCase("")]
        [TestCase("subCommand")]
        [TestCase("subCommand WithArguments")]
        public void DefaultCallHandler_ShouldBeCalledWithArgs_WhenCommandIsTriggered(string arguments)
        {
            var args = TriggerCommand(arguments);

            _sut.Verify(p =>
                p.DefaultCallHandler(
                    It.IsAny<IPlayer>(),
                    It.IsAny<int>(),
                    args),
                Times.Once);
        }

        #endregion

        #region HasSubCommand: [Good Path]

        [Test]
        public void HasSubCommand_ShouldReturnSelf_WhenCalled()
        {
            var self = _sut.Object.HasSubCommand("_", x => x.Build());
            self.Should().Be(_sut.Object);
        }

        [Test]
        public void HasSubCommand_ShouldAddSubCommandToList_WhenCalled()
        {
            const string subCommandName = "subCommandName";

            _sut.Object.HasSubCommand(subCommandName, x => x.Build());

            _sut.Object.As<IHaveSubCommands>().SubCommands.Should().HaveCount(1);
            _sut.Object.As<IHaveSubCommands>().SubCommands.Should().ContainKey(subCommandName);
        }

        #endregion

        #region FluentChatCommand: [Good Path] Commmand Call Flow

        [Test]
        public void FluentChatCommand_ShouldCallDefaultHandler_WhenNoArgumentsPassed()
        {
            _sut.Object.HasSubCommand("subCommand", x => x.Build());
            var args = TriggerCommand();

            _sut.Verify(p =>
                    p.DefaultCallHandler(
                        It.IsAny<IPlayer>(),
                        It.IsAny<int>(),
                        args),
                Times.Once);
        }

        [Test]
        public void FluentChatCommand_ShouldCallDefaultHandler_WhenFirstArgumentIsNotASubCommand()
        {
            _sut.Object.HasSubCommand("subCommand", x => x.Build());
            var args = TriggerCommand("someArgument");

            _sut.Verify(p =>
                    p.DefaultCallHandler(
                        It.IsAny<IPlayer>(),
                        It.IsAny<int>(),
                        args),
                Times.Once);
        }

        [Test]
        public void FluentChatCommand_ShouldCallSubCommand_WhenValidArgumentPassed()
        {
            _sut.Object.HasSubCommand(SubCommandName, _ => _sutSubCommand.Object.Build());
            var args = TriggerCommand(SubCommandName);

            _sutSubCommand.Verify(p =>
                    p.DefaultCallHandler(
                        It.IsAny<IPlayer>(),
                        It.IsAny<int>(),
                        args),
                Times.Once);
        }

        [TestCase]
        [TestCase("a")]
        [TestCase("a", "b")]
        [TestCase("a", "b", "c")]
        [TestCase("a", "b", "c", "d")]
        public void FluentChatCommand_ShouldDisplayCorrectSyntaxMessage_WhenDisplayingHelp(params string[] subCommands)
        {
            var commandName = $"{Guid.NewGuid()}";
            var command = FluentChat.RegisterCommand(commandName, _mockClientApi.Api)!;
            var concreteCommand = (FluentChatCommand)command;

            foreach (var subCommand in subCommands)
            {
                command.HasSubCommand(subCommand, x => x.Build());
            }

            var syntaxMessage = subCommands.Length > 0 ? $"[{string.Join("|", subCommands)}]" : "";
            var expectedMessage = $"{commandName}: {concreteCommand.Command.Description}\nSyntax: {syntaxMessage}";

            concreteCommand.Command.GetHelpMessage().Should().Match(expectedMessage);
        }

        #endregion
    }
}
#endif