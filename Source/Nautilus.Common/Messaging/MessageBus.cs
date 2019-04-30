// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : ReceiveActor
        where T : Message
    {
        private readonly ILogger log;
        private readonly IEndpoint messageStore;
        private readonly CommandHandler commandHandler;

        private Switchboard switchboard;
        private int messageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageStore">The message store endpoint.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MessageBus(
            IComponentryContainer container,
            IEndpoint messageStore)
        {
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(messageStore, nameof(messageStore));

            this.log = container.LoggerFactory.Create(NautilusService.Messaging, new Label($"{typeof(T).Name}Bus"));
            this.messageStore = messageStore;
            this.commandHandler = new CommandHandler(this.log);

            this.Receive<InitializeSwitchboard>(this.OnMessage);
            this.Receive<Envelope<T>>(this.OnReceive);
        }

        /// <summary>
        /// Runs pre-start of the receive actor.
        /// </summary>
        protected override void PreStart()
        {
            this.log.Debug($"{typeof(T).Name}Bus initializing...");
        }

        /// <summary>
        /// Logs unhandled messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void Unhandled(object message)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement (if else is clearer).
            if (message is null)
            {
                message = "NULL";
            }
            else if (message.Equals(string.Empty))
            {
                message = "EMPTY_STRING";
            }

            this.log.Warning($"Unhandled message {message}.");
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            Debug.NotNull(message.Switchboard, nameof(message.Switchboard));

            this.commandHandler.Execute(() =>
            {
                this.switchboard = message.Switchboard;

                this.log.Information($"{this.switchboard.GetType().Name} initialized.");
            });
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.commandHandler.Execute(() =>
            {
                this.messageCount++;
                this.messageStore.Send(envelope);

                this.log.Verbose($"[{this.messageCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
            });
        }

        private void OnReceive(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.commandHandler.Execute(() =>
            {
                this.switchboard.SendToReceiver(envelope);

                this.LogEnvelope(envelope);
            });
        }
    }
}
