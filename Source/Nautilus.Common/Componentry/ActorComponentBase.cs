//--------------------------------------------------------------
// <copyright file="ActorComponentBase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components which are also Akka.NET actors.
    /// </summary>
    public abstract class ActorComponentBase : ReceiveActor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorComponentBase"/> class.
        /// </summary>
        /// <param name="service">The nautilus service name.</param>
        /// <param name="component">The component label.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        protected ActorComponentBase(
            Enum service,
            Label component,
            ComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(setupContainer, nameof(setupContainer));

            this.Service = service;
            this.Component = component;
            this.Clock = setupContainer.Clock;
            this.Logger = setupContainer.LoggerFactory.Create(this.Service, this.Component);
            this.GuidFactory = setupContainer.GuidFactory;
            this.CommandHandler = new CommandHandler(this.Logger);
            this.MessagingAdapter = messagingAdapter;

            this.SetupMessageHandling();
        }

        /// <summary>
        /// Gets the components black box service context.
        /// </summary>
        protected Enum Service { get; }

        /// <summary>
        /// Gets the components label.
        /// </summary>
        protected Label Component { get; }

        /// <summary>
        /// Gets the black box clock.
        /// </summary>
        protected IZonedClock Clock { get; }

        /// <summary>
        /// Gets the black box logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the black box <see cref="Guid"/> factory.
        /// </summary>
        protected IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the command handler.
        /// </summary>
        protected CommandHandler CommandHandler { get; }

        /// <summary>
        /// Gets the components messaging adapter.
        /// </summary>
        protected IMessagingAdapter MessagingAdapter { get; }

        /// <summary>
        /// Sends the given message to the given black box service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        protected void SendMessage(
            Enum receiver,
            Message message)
        {
            Validate.NotNull(message, nameof(message));

            this.MessagingAdapter.Send(receiver, message, this.Service);
        }

        /// <summary>
        /// Sends the given messages to the given list of black box services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected void SendMessage(
            IReadOnlyCollection<Enum> receivers,
            Message message)
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            this.MessagingAdapter.Send(receivers, message, this.Service);
        }

        /// <summary>
        /// Pre start method when actor base class is called.
        /// </summary>
        protected override void PreStart()
        {
            this.Logger.Log(LogLevel.Debug, $"{this} initializing...");
        }

        /// <summary>
        /// Post restart method when the actor base class is restarted.
        /// </summary>
        /// <param name="reason">The restart reason.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected override void PostRestart(Exception reason)
        {
            Validate.NotNull(reason, nameof(reason));

            this.Logger.Log(LogLevel.Information, $"{this} restarting ({reason.Message})");
            this.Logger.LogException(reason);

            this.PreStart();
        }

        /// <summary>
        /// Logs unhandled messages sent to this actor.
        /// </summary>
        /// <param name="message">The message object.</param>
        protected override void Unhandled([CanBeNull] object message)
        {
            this.Logger.Log(LogLevel.Error, $"Unhandled message {message}");
        }

        /// <summary>
        /// Creates a log event with the given level and text.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        protected void Log(LogLevel logLevel, [CanBeNull] string logText)
        {
            this.Logger.Log(logLevel, logText);
        }

        /// <summary>
        /// Returns the current time of the black box system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow()
        {
            return this.Clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid()
        {
            return this.GuidFactory.NewGuid();
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
        private void Open<T>(Envelope<T> envelope) where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            var message = envelope.Open(this.Clock.TimeNow());
            this.Self.Tell(message);

            this.Log(LogLevel.Debug, $"Received {message}");
        }
    }
}
