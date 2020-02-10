//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusServiceBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Service
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public abstract class NautilusServiceBase : MessageBusConnected
    {
        private readonly MessageBusAdapter messageBus;
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledConnect;
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledDisconnect;
        private readonly List<Address> connectionAddresses = new List<Address>();

        private ZonedDateTime nextConnectTime;
        private ZonedDateTime nextDisconnectTime;
        private bool maintainConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusServiceBase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        protected NautilusServiceBase(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            IScheduler scheduler,
            FixConfiguration config)
            : base(container, messageBusAdapter)
        {
            this.messageBus = messageBusAdapter;
            this.Scheduler = scheduler;
            this.scheduledConnect = config.ConnectTime;
            this.scheduledDisconnect = config.DisconnectTime;
            this.nextConnectTime = TimingProvider.GetNextUtc(
                this.scheduledConnect.Day,
                this.scheduledConnect.Time,
                this.InstantNow());
            this.nextDisconnectTime = TimingProvider.GetNextUtc(
                this.scheduledDisconnect.Day,
                this.scheduledDisconnect.Time,
                this.InstantNow());
            this.maintainConnection = false;

            // Commands
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Events
            this.RegisterHandler<SessionConnected>(this.OnMessage);
            this.RegisterHandler<SessionDisconnected>(this.OnMessage);

            // Event Subscriptions
            this.Subscribe<SessionConnected>();
            this.Subscribe<SessionDisconnected>();
        }

        /// <summary>
        /// Gets the services scheduler.
        /// </summary>
        protected IScheduler Scheduler { get; }

        /// <summary>
        /// Register the given address to receiver generated connect messages.
        /// </summary>
        /// <param name="receiver">The receiver to register.</param>
        protected void RegisterConnectionAddress(Address receiver)
        {
            if (!this.connectionAddresses.Contains(receiver))
            {
                this.connectionAddresses.Add(receiver);
            }
            else
            {
                this.Log.Warning($"Connection address {receiver} was already registered.");
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsInsideInterval(
                this.scheduledDisconnect,
                this.scheduledConnect,
                this.InstantNow()))
            {
                // Inside disconnection schedule weekly interval
                this.CreateConnectFixJob();
                this.CreateDisconnectFixJob();
            }
            else
            {
                // Outside disconnection schedule weekly interval
                this.CreateDisconnectFixJob();
                this.CreateConnectFixJob();
            }

            this.OnServiceStart(start);

            if (this.IsTimeToConnect())
            {
                this.Send(start, this.connectionAddresses);
            }

            this.Log.Information("Running...");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.maintainConnection = false;  // Avoid immediate reconnection
            this.Send(stop, this.connectionAddresses);
            this.OnServiceStop(stop);
            this.messageBus.Stop();
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        /// <param name="start">The start message.</param>
        protected virtual void OnServiceStart(Start start)
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Stop"/> message.
        /// </summary>
        /// <param name="stop">The start message.</param>
        protected virtual void OnServiceStop(Stop stop)
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        protected virtual void OnConnected()
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            // Do nothing if not overridden
        }

        private bool IsTimeToConnect()
        {
            var connectThisWeek = TimingProvider.ThisWeek(this.scheduledConnect, this.InstantNow());
            var disconnectThisWeek = TimingProvider.ThisWeek(this.scheduledDisconnect, this.InstantNow());

            var now = this.TimeNow();
            return now.IsLessThan(disconnectThisWeek) || now.IsGreaterThan(connectThisWeek);
        }

        private void OnMessage(Connect connect)
        {
            // Forward message
            this.maintainConnection = true;
            this.Send(connect, this.connectionAddresses);
        }

        private void OnMessage(Disconnect disconnect)
        {
            // Forward message
            this.maintainConnection = false;  // Avoid immediate reconnection
            this.Send(disconnect, this.connectionAddresses);
        }

        private void OnMessage(SessionConnected message)
        {
            this.Log.Information($"Connected to session {message.SessionId}.");

            if (this.nextDisconnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateDisconnectFixJob();
            }

            this.OnConnected();
        }

        private void OnMessage(SessionDisconnected message)
        {
            if (this.maintainConnection)
            {
                this.Log.Warning($"Disconnected from session {message.SessionId}.");
            }
            else
            {
                this.Log.Information($"Disconnected from session {message.SessionId}.");
            }

            if (this.nextConnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateConnectFixJob();
            }

            this.OnDisconnected();
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.scheduledConnect.Day,
                this.scheduledConnect.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new Connect(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.Scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextConnectTime = nextTime;

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.scheduledDisconnect.Day,
                this.scheduledDisconnect.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new Disconnect(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.Scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextDisconnectTime = nextTime;

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }
    }
}
