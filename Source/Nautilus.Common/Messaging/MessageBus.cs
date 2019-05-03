// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using NautilusMQ;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : ComponentBase
        where T : Message
    {
        private readonly ILogger log;
        private readonly IEndpoint messageStorer;
        private readonly CommandHandler commandHandler;

        private Switchboard switchboard;
        private int messageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageStorer">The message storer endpoint.</param>
        public MessageBus(IComponentryContainer container, IEndpoint messageStorer)
        : base(
            NautilusService.Messaging,
            new Label("MessageBus"),
            container)
        {
            this.log = container.LoggerFactory.Create(NautilusService.Messaging, new Label($"{typeof(T).Name}Bus"));
            this.messageStorer = messageStorer;
            this.commandHandler = new CommandHandler(this.log);
            this.switchboard = Switchboard.Empty();
        }

        /// <summary>
        /// Runs pre-start of the receive actor.
        /// </summary>
        public override void Start()
        {
            this.log.Debug($"{typeof(T).Name}Bus initializing...");
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.commandHandler.Execute(() =>
            {
                this.switchboard = message.Switchboard;

                this.log.Information($"{this.switchboard.GetType().Name} initialized.");
            });
        }

        private void OnMessage(Envelope<T> envelope)
        {
            this.commandHandler.Execute(() =>
            {
                this.switchboard.SendToReceiver(envelope);
                this.LogEnvelope(envelope);
            });
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            this.commandHandler.Execute(() =>
            {
                this.messageCount++;
                this.messageStorer.Send(envelope);

                this.log.Verbose($"[{this.messageCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
            });
        }
    }
}
