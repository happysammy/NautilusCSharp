//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusServiceBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Service
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Scheduling;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public abstract class NautilusServiceBase : MessageBusConnected
    {
        private readonly MessageBusAdapter messageBus;
        private readonly List<Address> connectionAddresses = new List<Address>();
        private readonly WeeklyTime connectWeeklyTime;
        private readonly WeeklyTime disconnectWeeklyTime;

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
            this.connectWeeklyTime = config.ConnectWeeklyTime;
            this.disconnectWeeklyTime = config.DisconnectWeeklyTime;
            this.nextConnectTime = TimingProvider.GetNextUtc(this.connectWeeklyTime, this.InstantNow());
            this.nextDisconnectTime = TimingProvider.GetNextUtc(this.disconnectWeeklyTime, this.InstantNow());
            this.maintainConnection = false;

            // Commands
            this.RegisterHandler<ConnectSession>(this.OnMessage);
            this.RegisterHandler<DisconnectSession>(this.OnMessage);

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
                this.Logger.LogWarning($"Connection address {receiver} was already registered.");
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.Logger.LogInformation($"Starting {this.GetType().Name}...");

            if (TimingProvider.IsInsideInterval(
                this.disconnectWeeklyTime,
                this.connectWeeklyTime,
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

            this.Logger.LogInformation("Running...");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.Logger.LogInformation($"Stopping {this.GetType().Name}...");

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
            var connectThisWeek = TimingProvider.ThisWeek(this.connectWeeklyTime, this.InstantNow());
            var disconnectThisWeek = TimingProvider.ThisWeek(this.disconnectWeeklyTime, this.InstantNow());

            var now = this.TimeNow();
            return now.IsLessThan(disconnectThisWeek) || now.IsGreaterThan(connectThisWeek);
        }

        private void OnMessage(ConnectSession connectSession)
        {
            // Forward message
            this.maintainConnection = true;
            this.Send(connectSession, this.connectionAddresses);
        }

        private void OnMessage(DisconnectSession disconnectSession)
        {
            // Forward message
            this.maintainConnection = false;  // Avoid immediate reconnection
            this.Send(disconnectSession, this.connectionAddresses);
        }

        private void OnMessage(SessionConnected message)
        {
            this.Logger.LogInformation($"Connected to session {message.SessionId}.");

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
                this.Logger.LogWarning($"Disconnected from session {message.SessionId}.");
            }
            else
            {
                this.Logger.LogInformation($"Disconnected from session {message.SessionId}.");
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
            var nextTime = TimingProvider.GetNextUtc(this.connectWeeklyTime, now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new ConnectSession(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.Scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextConnectTime = nextTime;

            this.Logger.LogInformation($"Created scheduled job {job} for {nextTime.ToIso8601String()}");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(this.disconnectWeeklyTime, now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new DisconnectSession(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.Scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextDisconnectTime = nextTime;

            this.Logger.LogInformation($"Created scheduled job {job} for {nextTime.ToIso8601String()}");
        }
    }
}
