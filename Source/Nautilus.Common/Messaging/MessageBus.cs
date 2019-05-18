// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System.Collections.Generic;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : MessagingAgent
        where T : Message
    {
        private readonly ILogger log;
        private readonly List<object> deadLetters;

        private Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MessageBus(IComponentryContainer container)
        {
            this.Name = new Label($"MessageBus<{typeof(T).Name}>");
            this.log = container.LoggerFactory.Create(NautilusService.Messaging, this.Name);
            this.deadLetters = new List<object>();
            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<Envelope<T>>(this.OnMessage);
            this.RegisterHandler<Stop>(this.OnStop);
            this.RegisterUnhandled(this.Unhandled);
        }

        /// <summary>
        /// Gets the name of the message bus.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the list of dead letters (unhandled messages).
        /// </summary>
        public IEnumerable<object> DeadLetters => this.deadLetters;

        private void OnStop(Stop message)
        {
            foreach (var letter in this.deadLetters)
            {
                this.log.Warning($"[DEAD LETTER] {letter}.");
            }
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.switchboard = message.Switchboard;
            this.switchboard.RegisterDeadLetterChannel(this.Unhandled);
            this.log.Information("Initialized.");
        }

        private void OnMessage(Envelope<T> envelope)
        {
            this.switchboard.SendToReceiver(envelope);
            this.LogEnvelope(envelope);
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            this.log.Verbose($"[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
        }

        private void Unhandled(object message)
        {
            this.log.Error($"Unhandled message [{message}].");
            this.deadLetters.Add(message);
        }
    }
}
