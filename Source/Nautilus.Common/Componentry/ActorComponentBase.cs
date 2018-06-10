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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components which are also Akka.NET actors.
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
            Enum service,
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

            this.SetupMessageHandling();
        }

        /// <summary>
        /// Gets the components service context.
        /// </summary>
        protected Enum Service { get; }

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
            this.Log.Warning($"Unhandled message {message}");
        }

        /// <summary>
        /// Pre start method when actor base class is called.
        /// </summary>
        protected override void PreStart()
        {
            this.Log.Debug($"Initializing...");
        }

        /// <summary>
        /// Post restart method when the actor base class is restarted.
        /// </summary>
        /// <param name="ex">The restart reason exception.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected override void PostRestart(Exception ex)
        {
            Validate.NotNull(ex, nameof(ex));

            this.Log.Error($"Restarting ({ex.Message})", ex);

            this.PreStart();
        }

        /// <summary>
        /// Sets up the message handling for this actor component.
        /// </summary>
        private void SetupMessageHandling()
        {
            this.Receive<Envelope<CommandMessage>>(envelope => this.Open(envelope));
            this.Receive<Envelope<EventMessage>>(envelope => this.Open(envelope));
            this.Receive<Envelope<DocumentMessage>>(envelope => this.Open(envelope));
        }

        /// <summary>
        /// Opens the envelope and then sends the message back to the actor component.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="envelope">The message envelope.</param>
        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            var message = envelope.Open(this.clock.TimeNow());
            this.Self.Tell(message);

            this.Log.Debug($"Received {message}");
        }
    }
}
