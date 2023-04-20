#if UNIT_TEST
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    public class FluentChatSubCommandTests
    {
        private MockClientApi _mockClientApi;
        private Mock<FluentChatCommand> _mockCommand;
        private FluentChatSubCommand<FluentChatCommand> _concreteSut;
        private Mock<FluentChatSubCommand<FluentChatCommand>> _mockSut;
        private static string RandomValidCommandName => Guid.NewGuid().ToString();
        private const string CommandName = "test";
        private const string SubCommandName = "testSubCommand";

        #region Initialisation

        [SetUp]
        public void SetupMocks()
        {
            _mockClientApi = new MockClientApi();
            var command = new ClientChatCommand { Command = CommandName };

            _mockCommand = new Mock<FluentChatCommand>(command, _mockClientApi.Api) { CallBase = true };

            _concreteSut = new FluentChatSubCommand<FluentChatCommand>(SubCommandName, _mockCommand.Object, _mockClientApi.Api);

            _mockSut = new Mock<FluentChatSubCommand<FluentChatCommand>>(
                SubCommandName, _mockCommand.Object, _mockClientApi.Api)
            { CallBase = true };

            _mockSut.Setup(p => p.DefaultCallHandler(It.IsAny<IPlayer>(), It.IsAny<int>(), It.IsAny<CmdArgs>()))
                .Verifiable();
        }

        [TearDown]
        public void Teardown()
        {
            FluentChat.ClearCommands(_mockClientApi.Api);
        }

        #endregion

        #region Boilerplate

        [Test]
        public void Method_Should_When()
        {
            Assert.Pass();
        }

        #endregion

        #region FluentChatSubCommand: [Good Path] Validate Newly Constructed Defaults

        [Test]
        public void FluentChatSubCommand_ShouldSetSubCommandNameAsAlias_WhenNewlyConstructed()
        {
            _mockSut.Object.Aliases.Should().HaveCount(1);
            _mockSut.Object.Aliases.Should().Contain(SubCommandName);
        }

        [Test]
        public void FluentChatSubCommand_ShouldSetParent_WhenNewlyConstructed()
        {
            _mockSut.Object.Parent.Should().Be(_mockCommand.Object);
        }

        #endregion

        #region WithAlias: [Bad Path]

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WithAlias_ShouldNotAddAlias_WhenPassedNullEmptyOrWhiteSpace(string alias)
        {
            _mockSut.Object.WithAlias(alias);
            _mockSut.Object.Aliases.Should().HaveCount(1);
            _mockSut.Object.Aliases.Should().NotContain(alias);
        }

        #endregion

        #region WithAlias: [Good Path]

        [TestCase("validAlias")]
        [TestCase("AnotherValidAlias123")]
        [TestCase("HelloWorld!")]
        public void WithAlias_ShouldAddAlias_WhenPassedValidString(string alias)
        {
            _concreteSut.WithAlias(alias);
            _concreteSut.Aliases.Should().HaveCount(2);
            _concreteSut.Aliases.Should().Contain(alias);
        }

        [Test]
        public void WithAlias_ShouldAddMultipleAliases_WhenCalledMoreThanOnce()
        {
            _concreteSut.WithAlias("Hello");
            _concreteSut.WithAlias("World");
            _concreteSut.Aliases.Should().HaveCount(3);
            _concreteSut.Aliases.Should().Contain("Hello");
            _concreteSut.Aliases.Should().Contain("World");
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

            _concreteSut.WithHandler(ThisIsTheNewHandler);

            _concreteSut.CallHandler.Method.Name.Should().Be(fieldInfo.Name);
        }

        [Test]
        public void WithHandler_ShouldNotSetDefaultHandler_WhenPassedValidHandlerTwice()
        {
            var fieldInfo = GetType().GetMethod(nameof(ThisIsTheNewHandler), BindingFlags.NonPublic | BindingFlags.Static)!;

            _concreteSut.WithHandler(ThisIsTheNewHandler);
            _concreteSut.WithHandler((_, _, _) => { });

            _concreteSut.CallHandler.Method.Name.Should().Be(fieldInfo.Name);
        }

        #endregion

        #region HasSubCommand: [Good Path]

        [Test]
        public void HasSubCommand_ShouldReturnSelf_WhenCalled()
        {
            var self = _concreteSut.HasSubCommand("_", x => x.Build());
            self.Should().Be(_concreteSut);
        }

        [Test]
        public void HasSubCommand_ShouldAddSubCommandToList_WhenCalled()
        {
            const string subCommandName = "subCommandName";

            _concreteSut.HasSubCommand(subCommandName, x => x.Build());

            _concreteSut.As<IHaveSubCommands>().SubCommands.Should().HaveCount(1);
            _concreteSut.As<IHaveSubCommands>().SubCommands.Should().ContainKey(subCommandName);
        }

        #endregion

        #region Build: [Good Path]

        [Test]
        public void Build_ShouldReturnParent_WhenCalled()
        {
            var parent = _concreteSut.Build();
            parent.Should().Be(_mockCommand.Object);
        }

        #endregion
    }
}
#endif