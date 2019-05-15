// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : MessagingAgent
        where T : Message
    {
        private readonly ILogger log;
        private readonly IEndpoint messageStorer;

        private Switchboard switchboard;
        private int messageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageStorer">The message storer endpoint.</param>
        public MessageBus(IComponentryContainer container, IEndpoint messageStorer)
        {
            this.Name = new Label($"MessageBus<{this.GetType().GetGenericTypeDefinition().Name}>");
            this.log = container.LoggerFactory.Create(NautilusService.Messaging, this.Name);
            this.messageStorer = messageStorer;
            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<Envelope<T>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the name of the message bus.
        /// </summary>
        public Label Name { get; }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.switchboard = message.Switchboard;

            this.log.Information($"Initialized.");
        }

        private void OnMessage(Envelope<T> envelope)
        {
            this.switchboard.SendToReceiver(envelope);
            this.LogEnvelope(envelope);
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            this.messageCount++;
            this.messageStorer.Send(envelope);

            this.log.Verbose($"[{this.messageCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
        }
    }
}
