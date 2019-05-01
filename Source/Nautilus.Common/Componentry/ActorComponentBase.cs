//--------------------------------------------------------------------------------------------------
// <copyright file="ActorComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components which inherit from the Akka.NET <see cref="ReceiveActor"/>.
    /// </summary>
    public abstract class ActorComponentBase : ReceiveActor
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly CommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorComponentBase"/> class.
        /// </summary>
        /// <param name="service">The nautilus service name.</param>
        /// <param name="component">The component label.</param>
        /// <param name="setupContainer">The setup container.</param>
        protected ActorComponentBase(
            NautilusService service,
            Label component,
            IComponentryContainer setupContainer)
        {
            this.clock = setupContainer.Clock;
            this.StartTime = this.clock.TimeNow();
            this.guidFactory = setupContainer.GuidFactory;
            this.Log = setupContainer.LoggerFactory.Create(service, component);
            this.commandHandler = new CommandHandler(this.Log);

            this.Receive<Envelope<Command>>(this.Open);
            this.Receive<Envelope<Event>>(this.Open);
            this.Receive<Envelope<Document>>(this.Open);

            // Command messages.
            this.Receive<StartSystem>(this.OnMessage);
            this.Receive<ShutdownSystem>(this.OnMessage);
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Gets the time the component was last started or reset.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        protected ZonedDateTime StartTime { get; }

        /// <summary>
        /// Returns the current time of the system clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        protected ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid() => this.guidFactory.NewGuid();

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="CommandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        /// <summary>
        /// Logs unhandled messages sent to this actor.
        /// </summary>
        /// <param name="message">The message object.</param>
        protected override void Unhandled(object message)
        {
            if (message is null)
            {
                message = "NULL";
            }
            else if (message.Equals(string.Empty))
            {
                message = "EMPTY_STRING";
            }

            this.Log.Warning($"Unhandled message {message}.");
        }

        /// <summary>
        /// Pre start method called when actor is initializing.
        /// </summary>
        protected override void PreStart()
        {
            this.Log.Debug("Initializing...");
        }

        /// <summary>
        /// Start method called when the <see cref="StartSystem"/> message is received.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void Start(StartSystem message)
        {
            // To implement - override method in subclass.
        }

        /// <summary>
        /// Post restart method called when the actor base class is restarted.
        /// </summary>
        /// <param name="ex">The restart reason exception.</param>
        protected override void PostRestart(Exception ex)
        {
            this.Log.Error($"Restarted from {ex.Message}.", ex);
        }

        /// <summary>
        /// Post stop method called when the actor base class is stopped.
        /// </summary>
        protected override void PostStop()
        {
            this.Log.Debug("Stopped.");
        }

        /// <summary>
        /// Handles system start messages.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage(StartSystem message)
        {
            this.Log.Debug($"Starting from {message}-{message.Id}...");
            this.Start(message);
        }

        /// <summary>
        /// Handles system shutdown messages.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage(ShutdownSystem message)
        {
            this.Log.Debug($"Shutting down from {message}-{message.Id}...");
            this.PostStop();
        }

        /// <summary>
        /// Opens the envelope and then sends the message back to the actor component.
        /// </summary>
        /// <param name="envelope">The message envelope.</param>
        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            var message = envelope.Open(this.clock.TimeNow());

            this.Self.Tell(message);

            this.Log.Verbose($"Received {message}.");
        }
    }
}
