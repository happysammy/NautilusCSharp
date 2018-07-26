// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    [Immutable]
    public sealed class MessageBus<T> : ReceiveActor
        where T : Message
    {
        private readonly IZonedClock clock;
        private readonly ILogger log;
        private readonly IActorRef messageStore;
        private readonly CommandHandler commandHandler;

        private Switchboard switchboard;
        private int messageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="container">The container.</param>
        /// <param name="messageStoreRef">The message store actor address.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MessageBus(
            Label component,
            IComponentryContainer container,
            IActorRef messageStoreRef)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messageStoreRef, nameof(messageStoreRef));

            this.clock = container.Clock;
            this.log = container.LoggerFactory.Create(NautilusService.Messaging, component);
            this.messageStore = messageStoreRef;
            this.commandHandler = new CommandHandler(this.log);

            this.SetupMessageHandling();
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
            this.log.Warning($"Unhandled message {message}");
        }

        /// <summary>
        /// Sets up all <see cref="Message"/> handling methods.
        /// </summary>
        private void SetupMessageHandling()
        {
            this.Receive<InitializeMessageSwitchboard>(msg => this.OnMessage(msg));
            this.Receive<Envelope<T>>(envelope => this.OnReceive(envelope));
        }

        private void OnMessage(InitializeMessageSwitchboard message)
        {
            Validate.NotNull(message.Switchboard, nameof(message.Switchboard));

            this.commandHandler.Execute(() =>
            {
                this.switchboard = message.Switchboard;

                this.log.Information($"{this.switchboard.GetType().Name} initialized");
            });
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.commandHandler.Execute(() =>
            {
                this.messageCount++;
                this.messageStore.Tell(envelope);

                foreach (var receiver in envelope.Receivers)
                {
                    this.log.Debug($"[{this.messageCount}] {envelope.Sender} -> {envelope} -> {receiver}");
                }
            });
        }

        private void OnReceive(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.commandHandler.Execute(() =>
            {
                this.switchboard.SendToReceivers(envelope);

                this.LogEnvelope(envelope);
            });
        }
    }
}
