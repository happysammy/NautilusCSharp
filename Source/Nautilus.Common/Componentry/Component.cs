//--------------------------------------------------------------------------------------------------
// <copyright file="Component.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    public abstract class Component : MessagingAgent
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly CommandHandler commandHandler;
        private readonly List<ZonedDateTime> startedTimes;
        private readonly List<ZonedDateTime> stoppedTimes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="container">The components componentry container.</param>
        /// <param name="initial">The initial state of the component.</param>
        protected Component(IComponentryContainer container, State initial = State.Init)
        {
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.commandHandler = new CommandHandler(this.Log);
            this.startedTimes = new List<ZonedDateTime>();
            this.stoppedTimes = new List<ZonedDateTime>();

            this.Name = new Label(this.CreateComponentName());
            this.Address = new Address(this.Name.ToString());
            this.Log = container.LoggerFactory.Create(this.Name);
            this.State = initial;

            this.RegisterHandler<Envelope<Start>>(this.Open);
            this.RegisterHandler<Envelope<Stop>>(this.Open);
            this.RegisterHandler<Start>(this.OnMessage);
            this.RegisterHandler<Stop>(this.OnMessage);
            this.RegisterUnhandled(this.Unhandled);

            this.InitializedTime = this.clock.TimeNow();
            this.Log.Debug("Initialized.");
        }

        /// <summary>
        /// Gets the components name.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the components messaging address.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// Gets the components current state.
        /// </summary>
        public State State { get; private set; }

        /// <summary>
        /// Gets the time the component was initialized.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime InitializedTime { get; }

        /// <summary>
        /// Gets the time the component was initialized.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public IReadOnlyList<ZonedDateTime> StartedTimes => this.startedTimes.ToList().AsReadOnly();

        /// <summary>
        /// Gets the time the component was initialized.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public IReadOnlyList<ZonedDateTime> StoppedTimes => this.stoppedTimes.ToList().AsReadOnly();

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Sends a new <see cref="Start"/> message to the component.
        /// </summary>
        public void Start()
        {
            this.SendToSelf(new Start(this.NewGuid(), this.TimeNow()));
        }

        /// <summary>
        /// Sends a new <see cref="Stop"/> message to the component.
        /// </summary>
        public void Stop()
        {
            this.SendToSelf(new Stop(this.NewGuid(), this.TimeNow()));
        }

        /// <summary>
        /// Actions to be performed on component start. Called when a Start message is received.
        /// After this method is called the component state with become Running.
        /// Note: A log event will be created for the Start message.
        /// </summary>
        /// <param name="start">The start message.</param>
        protected virtual void OnStart(Start start)
        {
            this.Log.Error($"Received {start} and OnStart() not overridden in implementation.");
        }

        /// <summary>
        /// Actions to be performed on component stop. Called when a Stop message is received.
        /// After this method is called the component state will become Stopped.
        /// Note: A log event will be created for the Stop message.
        /// </summary>
        /// <param name="stop">The stop message.</param>
        protected virtual void OnStop(Stop stop)
        {
            this.Log.Error($"Received {stop} and OnStop() not overridden in implementation.");
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

        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            this.SendToSelf(envelope.Message);
        }

        private void OnMessage(Start message)
        {
            this.Log.Information($"Starting from message {message}...");

            this.OnStart(message);
            this.startedTimes.Add(this.TimeNow());
            this.State = State.Running;

            this.Log.Information($"{this.State}...");
        }

        private void OnMessage(Stop message)
        {
            this.Log.Information($"Stopping from message {message}...");

            this.OnStop(message);
            this.stoppedTimes.Add(this.TimeNow());
            this.State = State.Stopped;

            this.Log.Information($"{this.State}.");
        }

        private void Unhandled(object message)
        {
            this.Log.Error($"Unhandled message [{message}].");
            this.AddToUnhandledMessages(message);
        }

        private string CreateComponentName()
        {
            var thisType = this.GetType();

            return thisType.IsGenericType
                ? $"{thisType.Name.Split('`')[0]}<{thisType.GenericTypeArguments[0].Name}>"
                : thisType.Name;
        }
    }
}
