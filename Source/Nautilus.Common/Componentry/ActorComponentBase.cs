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
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.CQS;
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
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;
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
            this.Component = component;
            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.logger = setupContainer.LoggerFactory.Create(this.Service, this.Component);
            this.commandHandler = new CommandHandler(this.logger);

            this.SetupMessageHandling();
        }

        /// <summary>
        /// Gets the components service context.
        /// </summary>
        protected Enum Service { get; }

        /// <summary>
        /// Gets the components name.
        /// </summary>
        protected Label Component { get; }

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
        /// Logs unhandled messages sent to this actor.
        /// </summary>
        /// <param name="message">The message object.</param>
        protected override void Unhandled([CanBeNull] object message)
        {
            this.logger.Log(LogLevel.Error, $"Unhandled message {message}");
        }

        /// <summary>
        /// Creates a log event with the given level and text.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        protected void Log(LogLevel logLevel, [CanBeNull] string logText)
        {
            this.logger.Log(logLevel, logText);
        }

        /// <summary>
        /// Logs the result with the <see cref="ILogger"/>.
        /// </summary>
        /// <param name="result">The command result.</param>
        protected void LogResult(ResultBase result)
        {
            if (result.IsSuccess)
            {
                this.Log(LogLevel.Information, result.Message);
            }
            else
            {
                this.Log(LogLevel.Warning, result.Message);
            }
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
        /// Pre start method when actor base class is called.
        /// </summary>
        protected override void PreStart()
        {
            this.logger.Log(LogLevel.Debug, $"{this} initializing...");
        }

        /// <summary>
        /// Post restart method when the actor base class is restarted.
        /// </summary>
        /// <param name="reason">The restart reason.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected override void PostRestart(Exception reason)
        {
            Validate.NotNull(reason, nameof(reason));

            this.logger.Log(LogLevel.Information, $"{this} restarting ({reason.Message})");
            this.logger.LogException(reason);

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
        private void Open<T>(Envelope<T> envelope) where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            var message = envelope.Open(this.clock.TimeNow());
            this.Self.Tell(message);

            this.Log(LogLevel.Debug, $"Received {message}");
        }
    }
}
