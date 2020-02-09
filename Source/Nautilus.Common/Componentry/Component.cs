//--------------------------------------------------------------------------------------------------
// <copyright file="Component.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// The base class for all service components.
    /// </summary>
    public abstract class Component : MessagingAgent
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly List<ZonedDateTime> startedTimes;
        private readonly List<ZonedDateTime> stoppedTimes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="container">The components componentry container.</param>
        /// <param name="initial">The initial state of the component.</param>
        protected Component(IComponentryContainer container, State initial = State.Initialized)
        {
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.startedTimes = new List<ZonedDateTime>();
            this.stoppedTimes = new List<ZonedDateTime>();

            this.Name = new Label(this.GetType().NameFormatted());
            this.Address = new Address(this.Name.Value);
            this.Mailbox = new Mailbox(this.Address, this.Endpoint);
            this.Log = container.LoggerFactory.Create(this.Name);
            this.State = initial;

            this.RegisterExceptionHandler(this.HandleException);
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
        /// Gets the components messaging mailbox.
        /// </summary>
        public Mailbox Mailbox { get; }

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
        /// Returns the current time of the service clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns the current instant of the service clock.
        /// </summary>
        /// <returns>
        /// An <see cref="Instant"/>.
        /// </returns>
        public Instant InstantNow() => this.clock.InstantNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        public Guid NewGuid() => this.guidFactory.Generate();

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
            // this.Log.Error($"Received {start} with OnStart() not overridden in implementation.");
            this.Log.Warning($"Received {start}.");
        }

        /// <summary>
        /// Actions to be performed on component stop. Called when a Stop message is received.
        /// After this method is called the component state will become Stopped.
        /// Note: A log event will be created for the Stop message.
        /// </summary>
        /// <param name="stop">The stop message.</param>
        protected virtual void OnStop(Stop stop)
        {
            // this.Log.Error($"Received {stop} with OnStop() not overridden in implementation.");
            this.Log.Warning($"Received {stop}.");
        }

        /// <summary>
        /// Opens the envelope and sends the contained message to self.
        /// </summary>
        /// <param name="envelope">The envelope to open.</param>
        protected void OnEnvelope(IEnvelope envelope)
        {
            this.SendToSelf(envelope.MessageBase);

            this.Log.Verbose($"Received {envelope}.");
        }

        private void OnMessage(Start message)
        {
            this.Log.Debug($"Starting from message {message}...");

            this.OnStart(message);
            this.startedTimes.Add(this.TimeNow());
            this.State = State.Running;

            this.Log.Information($"{this.State}...");
        }

        private void OnMessage(Stop message)
        {
            this.Log.Debug($"Stopping from message {message}...");

            this.OnStop(message);
            this.stoppedTimes.Add(this.TimeNow());
            this.State = State.Stopped;

            this.Log.Information($"{this.State}.");
        }

        private void HandleException(Exception ex)
        {
            switch (ex)
            {
                case NullReferenceException _:
                case ArgumentException _:
                    this.Log.Error(ex.Message, ex);
                    break;
                default:
                    this.Log.Fatal(ex.Message, ex);

                    throw ex;
            }
        }

        private void Unhandled(object message)
        {
            this.Log.Error($"Unhandled message: {message}.");
            this.AddToUnhandledMessages(message);
        }
    }
}
