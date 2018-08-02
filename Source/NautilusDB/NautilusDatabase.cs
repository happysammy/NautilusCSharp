//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Contains the Nautilus Database system.
    /// </summary>
    public sealed class NautilusDatabase : ComponentBusConnectedBase
    {
        private readonly ActorSystem actorSystem;
        private readonly string actorSystemName;
        private readonly Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusDatabase"/> class.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="switchboard">The switchboard.</param>
        public NautilusDatabase(
            ActorSystem actorSystem,
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Switchboard switchboard)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(NautilusDatabase)),
                container,
                messagingAdapter)
        {
            this.actorSystem = actorSystem;
            this.actorSystemName = this.actorSystem.Name;
            this.switchboard = switchboard;

            var initializeSwitchboard =
                new InitializeSwitchboard(
                    switchboard,
                    this.NewGuid(),
                    this.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);
        }

        /// <summary>
        /// Shuts down the <see cref="NautilusDatabase"/> system.
        /// </summary>
        public void Shutdown()
        {
//            var shutdownTasks = this.addresses.Select(
//                    address => address.Value.GracefulStop(Duration.FromSeconds(10)))
//                .Cast<Task>()
//                .ToList();
//
//            this.Log.Information($"Waiting for actors to shut down...");
//            Task.WhenAll(shutdownTasks);

            this.Log.Information($"{this.actorSystemName} ActorSystem shutting down...");
            this.actorSystem.Terminate();
            this.Log.Information($"{this.actorSystemName} terminated.");

            this.Dispose();
        }

        /// <summary>
        /// Disposes the <see cref="DataService"/> object.
        /// </summary>
        private void Dispose()
        {
            this.actorSystem?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
