#if UNIT_TEST
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ApacheTech.VintageMods.FluentChatCommands.Abstractions;
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
    public class FluentChatTests
    {
        private MockClientApi _mockClientApi;
        private MockServerApi _mockServerApi;
        private static string RandomValidCommandName => Guid.NewGuid().ToString();

        #region Initialisation

        [SetUp]
        public void SetupMocks()
        {
            _mockClientApi = new MockClientApi();
            _mockServerApi = new MockServerApi();
        }

        [TearDown]
        public void Teardown()
        {
            FluentChat.ClearCommands(_mockClientApi.Api);
            FluentChat.ClearCommands(_mockServerApi.Api);
        }

        #endregion

        #region RegisterCommand: [Bad Path] Invalid Parameters 

        [Test]
        public void RegisterCommand_ShouldThrowArgumentNullException_WhenPassedNullApi()
        {
            var mockCall = new Action(() =>
            {
                FluentChat.RegisterCommand(
                    RandomValidCommandName, null!);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void RegisterCommand_ShouldThrowArgumentNullException_WhenPassedAllInvalidParameters(
            string commandName)
        {
            var mockCall = new Action(() =>
            {
                FluentChat.RegisterCommand(
                    commandName, null!);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void RegisterCommand_ShouldLogError_WhenPassedInvalidString(
            string commandName)
        {
            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            _mockClientApi.Mock.Verify(p =>
                p.Logger.Error(It.IsAny<string>()),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void RegisterCommand_ShouldReturnNull_WhenPassedInvalidString(
            string commandName)
        {
            var actual = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            actual.Should().BeNull();
        }

        #endregion

        #region RegisterCommand: [Bad Path] Client Command Registration

        [Test]
        public void RegisterCommand_ShouldReturnNull_WhenAlreadyRegisteredOnClient()
        {
            var commandName = RandomValidCommandName;

            var actual = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var duplicate = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            actual.Should().NotBeNull();
            duplicate.Should().BeNull();
        }

        [Test]
        public void RegisterCommand_ShouldLogError_WhenCommandAlreadyRegisteredOnClient()
        {
            var commandName = RandomValidCommandName;

            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            _mockClientApi.Mock.Verify(p =>
                    p.Logger.Error(It.IsAny<string>()),
                Times.Once);
        }

        #endregion

        #region RegisterCommand: [Good Path] Client Command Registration

        [Test]
        public void RegisterCommand_ShouldRegisterCommandWithApi_WhenNotAlreadyRegisteredOnClient()
        {
            var commandName = RandomValidCommandName;

            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            _mockClientApi.Mock.Verify(p =>
                p.RegisterCommand(It.IsAny<ClientChatCommand>()),
                Times.Once);
        }

        #endregion

        #region RegisterCommand: [Bad Path] Server Command Registration

        [Test]
        public void RegisterCommand_ShouldReturnNull_WhenAlreadyRegisteredOnServer()
        {
            var commandName = RandomValidCommandName;

            var actual = FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            var duplicate = FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            actual.Should().NotBeNull();
            duplicate.Should().BeNull();
        }

        [Test]
        public void RegisterCommand_ShouldLogError_WhenCommandAlreadyRegisteredOnServer()
        {
            var commandName = RandomValidCommandName;

            FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            _mockServerApi.Mock.Verify(p =>
                    p.Logger.Error(It.IsAny<string>()),
                Times.Once);
        }

        #endregion

        #region RegisterCommand: [Good Path] Server Command Registration

        [Test]
        public void RegisterCommand_ShouldRegisterCommandWithApi_WhenNotAlreadyRegisteredOnServer()
        {
            var commandName = RandomValidCommandName;

            FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            _mockServerApi.Mock.Verify(p =>
                    p.RegisterCommand(It.IsAny<ServerChatCommand>()),
                Times.Once);
        }

        #endregion

        #region RegisterCommand: [Good Path] Server and Client Command Registration

        [Test]
        public void RegisterCommand_ShouldRegisterCommandWithTheSameName_WhenCalledFromBothAppSides()
        {
            var commandName = RandomValidCommandName;

            var clientCommand = FluentChat.RegisterCommand(commandName, _mockClientApi!.Api);
            var serverCommand = FluentChat.RegisterCommand(commandName, _mockServerApi!.Api);

            var concreteClientCommand = (FluentChatCommand)clientCommand;
            var concreteServerCommand = (FluentChatCommand)serverCommand;

            _mockClientApi.Mock.Verify(p =>
                    p.RegisterCommand(It.IsAny<ClientChatCommand>()),
                Times.Once);

            _mockServerApi.Mock.Verify(p =>
                    p.RegisterCommand(It.IsAny<ServerChatCommand>()),
                Times.Once);

            FluentChat.CachedCommands.Should().Contain(concreteClientCommand);
            FluentChat.CachedCommands.Should().Contain(concreteServerCommand);
        }

        #endregion

        #region RegisterCommand: [Bad Path] Command Caching

        [Test]
        public void RegisterCommand_ShouldNotAddCommandToCache_WhenAlreadyExists()
        {
            var countBefore = FluentChat.CachedCommands.Count;
            var commandName = RandomValidCommandName;

            var actual = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var concreteActual = actual as FluentChatCommand;

            var duplicate = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var concreteDuplicate = duplicate as FluentChatCommand;

            FluentChat.CachedCommands.Count
                .Should().Be(countBefore + 1);

            FluentChat.CachedCommands
                .Should().Contain(concreteActual);

            FluentChat.CachedCommands
                .Should().NotContain(concreteDuplicate);
        }

        [Test]
        public void RegisterCommand_ShouldReturnNull_WhenCommandAlreadyExistsInCache()
        {
            var commandName = RandomValidCommandName;

            var valid = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var invalid = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            valid.Should().NotBeNull();
            invalid.Should().BeNull();
        }

        #endregion

        #region RegisterCommand: [Good Path] Command Caching

        [Test]
        public void RegisterCommand_ShouldAddCommandToCache_WhenNotAlreadyPresent()
        {
            var countBefore = FluentChat.CachedCommands.Count;
            var commandName = RandomValidCommandName;

            var actual = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var concreteActual = actual as FluentChatCommand;

            FluentChat.CachedCommands.Count
                .Should().Be(countBefore + 1);

            FluentChat.CachedCommands.Should()
                .Contain(concreteActual);
        }

        #endregion

        #region RegisterCommand: [Good Path] Valid Client Command 

        [Test]
        public void RegisterCommand_ShouldReturnIFluentChatCommand_WhenAllPathsSucceed()
        {
            var actual = FluentChat.RegisterCommand(
                RandomValidCommandName, _mockClientApi!.Api);

            actual.Should().NotBeNull();
            actual.Should().BeAssignableTo<IFluentChatCommand>();
        }

        #endregion

        #region ClientCommand: [Bad Path] Get Command From Cache

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("ValidCommandName")]
        public void ClientCommand_ShouldThrowKeyNotFoundException_WhenNotPresentInCache(
            string commandName)
        {
            var mockCall = new Action(() =>
            {
                FluentChat.ClientCommand(commandName);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<KeyNotFoundException>();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("ValidCommandName")]
        public void ClientCommand_ShouldThrowKeyNotFoundException_WhenPresentInServerCache(
            string commandName)
        {
            FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            var mockCall = new Action(() =>
            {
                FluentChat.ClientCommand(commandName);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<KeyNotFoundException>();
        }

        #endregion

        #region ClientCommand: [Good Path] Get Command From Cache

        [Test]
        public void ClientCommand_ShouldReturnCommand_WhenPresentInCache()
        {
            var commandName = RandomValidCommandName;
            var expected = FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var actual = FluentChat.ClientCommand(commandName);

            actual.Should().Be(expected);
        }

        #endregion

        #region GetCachedCommand: [Good Path] Case Insensitivity

        [Test]
        public void GetCachedCommand_ShouldIgnoreCase_WhenScanningForCommands()
        {
            const string commandName = "HelloWorld";
            FluentChat.RegisterCommand(commandName, _mockClientApi!.Api);

            var control = FluentChat.ClientCommand(commandName);
            var concreteControl = (FluentChatCommand)control;
            concreteControl.Command.Command.Should().Be(commandName.ToLowerInvariant());

            var testCall = new System.Func<string, FluentChatCommand>(input => (FluentChatCommand)FluentChat.ClientCommand(input));

            testCall.Invoking(p => p(commandName.ToUpperInvariant())).Should().NotThrow<KeyNotFoundException>();
            testCall.Invoking(p => p(commandName.ToLowerInvariant())).Should().NotThrow<KeyNotFoundException>();
        }

        #endregion

        #region ServerCommand: [Bad Path] Get Command From Cache

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("ValidClientCommandName")]
        public void ServerCommand_ShouldThrowKeyNotFoundException_WhenNotPresentInCache(
            string commandName)
        {
            var mockCall = new Action(() =>
            {
                FluentChat.ServerCommand(commandName);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<KeyNotFoundException>();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("ValidServerCommandName")]
        public void ServerCommand_ShouldThrowKeyNotFoundException_WhenPresentInClientCache(
            string commandName)
        {
            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            var mockCall = new Action(() =>
            {
                FluentChat.ServerCommand(commandName);
            });

            mockCall
                .Invoking(p => p())
                .Should()
                .Throw<KeyNotFoundException>();
        }

        #endregion

        #region ServerCommand: [Good Path] Get Command From Cache

        [Test]
        public void ServerCommand_ShouldReturnCommand_WhenPresentInCache()
        {
            var commandName = RandomValidCommandName;
            var expected = FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            var actual = FluentChat.ServerCommand(commandName);

            actual.Should().Be(expected);
        }

        #endregion

        #region UnregisterCommand: [Good Path] Unregister Single Command

        [Test]
        public void UnregisterCommand_ShouldRemoveClientCommand_WhenCalledOnClient()
        {
            var countBefore = FluentChat.CachedCommands.Count;

            var commandName = RandomValidCommandName;
            FluentChat.RegisterCommand(
                commandName, _mockClientApi!.Api);

            FluentChat.UnregisterCommand(
                commandName, _mockClientApi!.Api);

            FluentChat.OnUnregisterCommand =
                _mockClientApi.OnUnregisterCommand;

            FluentChat.CachedCommands.Count
                .Should().Be(countBefore);
        }

        [Test]
        public void UnregisterCommand_ShouldNotRemoveClientCommand_WhenCalledOnServer()
        {
            // Arrange
            var commandName = RandomValidCommandName;

            FluentChat.OnUnregisterCommand = _mockServerApi!.OnUnregisterCommand;
            FluentChat.RegisterCommand(RandomValidCommandName, _mockServerApi!.Api);
            FluentChat.RegisterCommand(RandomValidCommandName, _mockServerApi!.Api);
            FluentChat.RegisterCommand(commandName, _mockClientApi!.Api);

            var clientCountBefore = _mockClientApi.RegisteredCommandCount;
            var serverCountBefore = _mockServerApi.RegisteredCommandCount;

            // Act
            FluentChat.UnregisterCommand(commandName, _mockServerApi!.Api);

            // Assert
            _mockClientApi.RegisteredCommandCount.Should().Be(clientCountBefore);
            _mockServerApi.RegisteredCommandCount.Should().Be(serverCountBefore);
            FluentChat.CachedCommands.Count.Should().Be(clientCountBefore + serverCountBefore);
        }

        [Test]
        public void UnregisterCommand_ShouldRemoveServerCommand_WhenCalledOnServer()
        {
            var countBefore = FluentChat.CachedCommands.Count;

            var commandName = RandomValidCommandName;
            FluentChat.RegisterCommand(
                commandName, _mockServerApi!.Api);

            FluentChat.UnregisterCommand(
                commandName, _mockServerApi!.Api);

            FluentChat.OnUnregisterCommand =
                _mockServerApi.OnUnregisterCommand;

            FluentChat.CachedCommands.Count
                .Should().Be(countBefore);
        }

        [Test]
        public void UnregisterCommand_ShouldNotRemoveServerCommand_WhenCalledOnClient()
        {
            // Arrange
            var commandName = RandomValidCommandName;

            FluentChat.OnUnregisterCommand = _mockClientApi!.OnUnregisterCommand;
            FluentChat.RegisterCommand(RandomValidCommandName, _mockClientApi!.Api);
            FluentChat.RegisterCommand(RandomValidCommandName, _mockClientApi!.Api);
            FluentChat.RegisterCommand(commandName, _mockServerApi!.Api);

            var clientCountBefore = _mockClientApi.RegisteredCommandCount;
            var serverCountBefore = _mockServerApi.RegisteredCommandCount;

            // Act
            FluentChat.UnregisterCommand(commandName, _mockClientApi!.Api);

            // Assert
            _mockClientApi.RegisteredCommandCount.Should().Be(clientCountBefore);
            _mockServerApi.RegisteredCommandCount.Should().Be(serverCountBefore);
            FluentChat.CachedCommands.Count.Should().Be(clientCountBefore + serverCountBefore);
        }

        #endregion

        #region ClearCommands: [Good Path] Clears All Commands

        [Test]
        public void ClearCommands_ShouldClearAllClientCommands_WhenCalledOnClient()
        {
            var mock = _mockClientApi!;
            var countBefore = mock.RegisteredCommandCount;
            FluentChat.OnUnregisterCommand = mock.OnUnregisterCommand;

            var actual1 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);
            var actual2 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);
            var actual3 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);

            mock.RegisteredCommandCount
                .Should().Be(countBefore + 3);

            FluentChat.CachedCommands
                .Should().Contain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual3 as FluentChatCommand);

            FluentChat.ClearCommands(mock.Api);

            mock.RegisteredCommandCount
                .Should().Be(0);

            FluentChat.CachedCommands
                .Should().NotContain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual3 as FluentChatCommand);
        }

        [Test]
        public void ClearCommands_ShouldClearAllServerCommands_WhenCalledOnServer()
        {
            var mock = _mockServerApi!;
            var countBefore = mock.RegisteredCommandCount;
            FluentChat.OnUnregisterCommand = mock.OnUnregisterCommand;

            var actual1 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);
            var actual2 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);
            var actual3 = FluentChat.RegisterCommand(
                RandomValidCommandName, mock.Api);

            mock.RegisteredCommandCount
                .Should().Be(countBefore + 3);

            FluentChat.CachedCommands
                .Should().Contain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual3 as FluentChatCommand);

            FluentChat.ClearCommands(mock.Api);

            mock.RegisteredCommandCount
                .Should().Be(0);

            FluentChat.CachedCommands
                .Should().NotContain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual3 as FluentChatCommand);
        }

        [Test]
        public void ClearCommands_ShouldNotClearAllServerCommands_WhenCalledOnClient()
        {
            var client = _mockClientApi!;
            var server = _mockServerApi!;
            var clientCountBefore = client.RegisteredCommandCount;
            var serverCountBefore = server.RegisteredCommandCount;
            FluentChat.OnUnregisterCommand = client.OnUnregisterCommand;

            var actual1 = FluentChat.RegisterCommand(
                RandomValidCommandName, client.Api);
            var actual2 = FluentChat.RegisterCommand(
                RandomValidCommandName, server.Api);
            var actual3 = FluentChat.RegisterCommand(
                RandomValidCommandName, server.Api);

            client.RegisteredCommandCount
                .Should().Be(clientCountBefore + 1);

            server.RegisteredCommandCount
                .Should().Be(serverCountBefore + 2);

            FluentChat.CachedCommands
                .Should().Contain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual3 as FluentChatCommand);

            FluentChat.ClearCommands(client.Api);

            client.RegisteredCommandCount
                .Should().Be(0);

            server.RegisteredCommandCount
                .Should().Be(2);

            FluentChat.CachedCommands
                .Should().NotContain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual3 as FluentChatCommand);
        }

        [Test]
        public void ClearCommands_ShouldNotClearAllClientCommands_WhenCalledOnServer()
        {
            var client = _mockClientApi!;
            var server = _mockServerApi!;
            var clientCountBefore = client.RegisteredCommandCount;
            var serverCountBefore = server.RegisteredCommandCount;
            FluentChat.OnUnregisterCommand = server.OnUnregisterCommand;

            var actual1 = FluentChat.RegisterCommand(
                RandomValidCommandName, client.Api);
            var actual2 = FluentChat.RegisterCommand(
                RandomValidCommandName, server.Api);
            var actual3 = FluentChat.RegisterCommand(
                RandomValidCommandName, server.Api);

            client.RegisteredCommandCount
                .Should().Be(clientCountBefore + 1);

            server.RegisteredCommandCount
                .Should().Be(serverCountBefore + 2);

            FluentChat.CachedCommands
                .Should().Contain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().Contain(actual3 as FluentChatCommand);

            FluentChat.ClearCommands(server.Api);

            client.RegisteredCommandCount
                .Should().Be(1);

            server.RegisteredCommandCount
                .Should().Be(0);

            FluentChat.CachedCommands
                .Should().Contain(actual1 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual2 as FluentChatCommand);

            FluentChat.CachedCommands
                .Should().NotContain(actual3 as FluentChatCommand);
        }

        #endregion
    }
}
#endif