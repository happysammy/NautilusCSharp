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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : Component
        where T : Message
    {
        private readonly List<object> deadLetters;

        private Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MessageBus(IComponentryContainer container)
        : base(container)
        {
            this.deadLetters = new List<object>();
            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<Envelope<T>>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the list of dead letters (unhandled messages).
        /// </summary>
        public IEnumerable<object> DeadLetters => this.deadLetters;

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            foreach (var letter in this.deadLetters)
            {
                this.Log.Warning($"[DEAD LETTER] {letter}.");
            }
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.switchboard = message.Switchboard;
            this.switchboard.RegisterDeadLetterChannel(this.AddToDeadLetters);

            this.Log.Information("Switchboard initialized.");
        }

        private void OnEnvelope(Envelope<T> envelope)
        {
            this.switchboard.SendToReceiver(envelope);

            this.Log.Verbose($"[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
        }

        private void AddToDeadLetters(object message)
        {
            this.deadLetters.Add(message);

            this.Log.Error($"Unhandled message [{message}].");
        }
    }
}
