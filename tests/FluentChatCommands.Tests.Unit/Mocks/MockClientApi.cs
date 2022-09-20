#if UNIT_TEST
using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.Extensions.System;
using ApacheTech.VintageMods.FluentChatCommands.Implementations;
using Moq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.FluentChatCommands.Tests.Unit.Mocks
{
    public class MockClientApi
    {
        private readonly Dictionary<string, ChatCommand> _registeredCommands = new();

        public Mock<ICoreClientAPI> Mock { get; }
        public ICoreClientAPI Api => Mock.Object;

        public int RegisteredCommandCount => _registeredCommands.Count;

        public MockClientApi()
        {
            Mock = new Mock<ICoreClientAPI>();
            Mock.Setup(p =>
                p.Logger.Error(It.IsAny<string>())).Verifiable();

            Mock.Setup(p =>
                p.ShowChatMessage(It.IsAny<string>())).Verifiable();

            Mock.Setup(p => p.World.Player).Returns(It.IsAny<IClientPlayer>());
            
            Mock.Setup(p => p.Side).Returns(EnumAppSide.Client);

            Mock.Setup(p =>
                    p.RegisterCommand(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ClientChatCommandDelegate>()))
                .Returns(MockClientRegistration)
                .Verifiable();

            Mock.Setup(p =>
                    p.RegisterCommand(
                        It.IsAny<ClientChatCommand>()))
                .Returns(MockClientCommandRegistration)
                .Verifiable();

            Mock.Setup(p => p.TriggerChatMessage(It.IsAny<string>())).Callback(TriggerCommand);
        }

        public void TriggerCommand(string chatMessage)
        {
            var args = new CmdArgs(chatMessage.Skip(1).ToString());
            var commandName = args.PopWord();
            var command = (FluentChatCommand)FluentChat.ClientCommand(commandName);
            var player = new Mock<IPlayer>();
            command.Command.CallHandler(player.Object, 0, args);
        }

        public void OnUnregisterCommand(string commandName)
        {
            _registeredCommands.RemoveIfPresent(commandName);
        }

        private bool MockClientCommandRegistration(
            ClientChatCommand command)
        {
            if (_registeredCommands.ContainsKey(command.Command))
            {
                return false;
            }
            _registeredCommands.Add(command.Command, command);
            return true;
        }

        private bool MockClientRegistration(
            string commandName,
            string description,
            string syntax,
            ClientChatCommandDelegate handler)
        {
            if (_registeredCommands.ContainsKey(commandName))
            {
                return false;
            }
            _registeredCommands.Add(commandName,
                new ClientChatCommand
                {
                    Command = commandName,
                    Description = description,
                    Syntax = syntax,
                    handler = handler,
                    RequiredPrivilege = null
                });
            return true;
        }
    }
}
#endif