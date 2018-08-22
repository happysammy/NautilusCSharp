//--------------------------------------------------------------------------------------------------
// <copyright file="ActorComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components which inherit from the Akka.NET <see cref="ReceiveActor"/>.
    /// </summary>
    [Stateless]
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
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(setupContainer, nameof(setupContainer));

            this.Service = service;
            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.Log = setupContainer.LoggerFactory.Create(service, component);
            this.commandHandler = new CommandHandler(this.Log);

            // Setup message handling.
            this.Receive<Envelope<Command>>(envelope => this.Open(envelope));
            this.Receive<Envelope<Event>>(envelope => this.Open(envelope));
            this.Receive<Envelope<Document>>(envelope => this.Open(envelope));
            this.Receive<SystemShutdown>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Gets the components service context.
        /// </summary>
        protected NautilusService Service { get; }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Returns the current time of the black box system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow()
        {
            return this.clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid()
        {
            return this.guidFactory.NewGuid();
        }

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
        protected override void Unhandled([CanBeNull] object message)
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

            this.Log.Warning($"Unhandled message {message}.");
        }

        /// <summary>
        /// Pre start method when actor base class is called.
        /// </summary>
        protected override void PreStart()
        {
            this.Log.Debug("Initializing...");
        }

        /// <summary>
        /// Post restart method when the actor base class is restarted.
        /// </summary>
        /// <param name="ex">The restart reason exception.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected override void PostRestart(Exception ex)
        {
            Validate.NotNull(ex, nameof(ex));

            this.Log.Error($"Restarting {ex.Message}...", ex);
            this.PreStart();
        }

        /// <summary>
        /// Post stop method when the actor base class is stopped.
        /// </summary>
        protected override void PostStop()
        {
            this.Log.Debug($"Stopped.");
        }

        private void OnMessage(SystemShutdown message)
        {
            this.Log.Information($"Shutting down from {message}-{message.Id}...");
            this.PostStop();
        }

        /// <summary>
        /// Opens the envelope and then sends the message back to the actor component.
        /// </summary>
        /// <param name="envelope">The message envelope.</param>
        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            var message = envelope.Open(this.clock.TimeNow());

            this.Self.Tell(message);

            this.Log.Debug($"Received {message}.");
        }
    }
}
