//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// The base class for all service components.
    /// </summary>
    public abstract class ComponentBase : MessagingAgent
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly CommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="serviceContext">The components service context.</param>
        /// <param name="container">The components componentry container.</param>
        protected ComponentBase(NautilusService serviceContext, IComponentryContainer container)
        {
            this.Name = new Label(this.CreateComponentName());
            this.Address = new Address(this.Name.ToString());
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.Log = container.LoggerFactory.Create(serviceContext, this.Name);
            this.commandHandler = new CommandHandler(this.Log);
            this.StartTime = this.clock.TimeNow();

            this.RegisterHandler<Envelope<Command>>(this.Open);
            this.RegisterHandler<Envelope<Event>>(this.Open);
            this.RegisterHandler<Envelope<Document>>(this.Open);

            this.RegisterHandler<Start>(this.OnMessage);
            this.RegisterHandler<Stop>(this.OnMessage);
            this.RegisterUnhandled(this.Unhandled);
        }

        /// <summary>
        /// Gets the components name label.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the components messaging address.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// Gets the time the component was last started.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime StartTime { get; }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Sends a <see cref="Start"/> message to the component.
        /// </summary>
        public void Start()
        {
            this.SendToSelf(new Start(this.NewGuid(), this.TimeNow()));
        }

        /// <summary>
        /// Sends a <see cref="Stop"/> message to the component.
        /// </summary>
        public void Stop()
        {
            this.SendToSelf(new Stop(this.NewGuid(), this.TimeNow()));
        }

        /// <summary>
        /// Handles the start message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void OnStart(Start message)
        {
            this.Log.Error($"Received {message} handling not implemented.");
        }

        /// <summary>
        /// Handles the stop message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void OnStop(Stop message)
        {
            this.Log.Error($"Received {message} handling not implemented.");
        }

        /// <summary>
        /// Returns the current time of the service clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns the current instant of the service clock.
        /// </summary>
        /// <returns>
        /// An <see cref="Instant"/>.
        /// </returns>
        protected Instant InstantNow() => this.clock.InstantNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid() => this.guidFactory.NewGuid();

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <typeparam name="T">The exception type.</typeparam>
        /// <param name="action">The action to execute.</param>
        protected void Execute<T>(Action action)
            where T : Exception
        {
            this.commandHandler.Execute<T>(action);
        }

        /// <summary>
        /// Calls the Start() virtual method.
        /// </summary>
        /// <param name="message">The start message.</param>
        private void OnMessage(Start message)
        {
            this.OnStart(message);
        }

        /// <summary>
        /// Calls the Stop() virtual method.
        /// </summary>
        /// <param name="message">The stop message.</param>
        private void OnMessage(Stop message)
        {
            this.OnStop(message);
        }

        private void Unhandled(object message)
        {
            this.Log.Error($"Unhandled message [{message}].");
            this.AddToUnhandledMessages(message);
        }

        /// <summary>
        /// Opens the envelope and then sends the message back to the actor component.
        /// </summary>
        /// <param name="envelope">The message envelope.</param>
        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            var message = envelope.Open(this.clock.TimeNow());
            this.Endpoint.Send(message);

            this.Log.Verbose($"Received {message}.");
        }

        private string CreateComponentName()
        {
            var thisType = this.GetType();

            if (thisType.IsGenericType)
            {
                return $"{thisType.Name.Split('`')[0]}<{thisType.GenericTypeArguments[0].Name}>";
            }

            return thisType.Name;
        }
    }
}
