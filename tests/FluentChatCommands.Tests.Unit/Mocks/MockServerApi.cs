#nullable enable
using System.Collections.Generic;
using ApacheTech.Common.Extensions.System;
using Moq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Mocks
{
    public class MockServerApi
    {
        private readonly Dictionary<string, ChatCommand> _registeredCommands = new();

        public Mock<ICoreServerAPI> Mock { get; }

        public ICoreServerAPI Api => Mock.Object;

        public int RegisteredCommandCount => _registeredCommands.Count;

        public MockServerApi()
        {
            Mock = new Mock<ICoreServerAPI>();
            Mock.Setup(p => p.Side).Returns(EnumAppSide.Server);
            Mock.Setup(p =>
                p.Logger.Error(It.IsAny<string>())).Verifiable();

            Mock.Setup(p =>
                    p.RegisterCommand(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ServerChatCommandDelegate>(),
                        It.IsAny<string?>()))
                .Returns(MockServerRegistration)
                .Verifiable();

            Mock.Setup(p =>
                    p.RegisterCommand(
                        It.IsAny<ServerChatCommand>()))
                .Returns(MockServerCommandRegistration)
                .Verifiable();
        }

        public void OnUnregisterCommand(string commandName)
        {
            _registeredCommands.RemoveIfPresent(commandName);
        }

        private bool MockServerCommandRegistration(ServerChatCommand command)
        {
            if (_registeredCommands.ContainsKey(command.Command))
            {
                return false;
            }
            _registeredCommands.Add(command.Command, command);
            return true;
        }

        private bool MockServerRegistration(
            string commandName,
            string description,
            string syntax,
            ServerChatCommandDelegate handler,
            string? privilege)
        {
            if (_registeredCommands.ContainsKey(commandName))
            {
                return false;
            }
            _registeredCommands.Add(commandName,
                new ServerChatCommand
                {
                    Command = commandName,
                    Description = description,
                    Syntax = syntax,
                    handler = handler,
                    RequiredPrivilege = privilege
                });
            return true;
        }
    }
}