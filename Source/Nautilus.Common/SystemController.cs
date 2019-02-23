//--------------------------------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common
{
    using System;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Address = Nautilus.Common.Messaging.Address;

    /// <summary>
    /// Provides a means of controlling services within the system.
    /// </summary>
    public class SystemController : ComponentBusConnectedBase
    {
        private readonly ActorSystem actorSystem;
        private readonly string actorSystemName;
        private readonly ReadOnlyList<Address> components;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="actorSystem">The systems actor system.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="switchboard">The switchboard.</param>
        public SystemController(
            IComponentryContainer container,
            ActorSystem actorSystem,
            MessagingAdapter messagingAdapter,
            Switchboard switchboard)
            : base(
                NautilusService.Core,
                LabelFactory.Create(nameof(SystemController)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(switchboard, nameof(switchboard));

            this.actorSystem = actorSystem;
            this.actorSystemName = actorSystem.Name;
            this.components = switchboard.Addresses;

            var initializeSwitchboard =
                new InitializeSwitchboard(
                    switchboard,
                    this.NewGuid(),
                    this.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);
        }

        /// <summary>
        /// Starts the <see cref="Nautilus"/> system.
        /// </summary>
        public void Start()
        {
            var start = new StartSystem(this.NewGuid(), this.TimeNow());

            foreach (var component in this.components)
            {
                this.Send(component, start);
            }
        }

        /// <summary>
        /// Shuts down the <see cref="Nautilus"/> system.
        /// </summary>
        public void Shutdown()
        {
            var shutdown = new ShutdownSystem(this.NewGuid(), this.TimeNow());

            foreach (var component in this.components)
            {
                this.Send(component, shutdown);
            }

            // Allow actor components to shutdown.
            Task.Delay(1000).Wait();
            this.ShutdownActorSystem();
        }

        private void ShutdownActorSystem()
        {
            this.Log.Information($"{this.actorSystemName} actor system shutting down...");
            this.actorSystem.Terminate();
            this.Log.Information($"{this.actorSystemName} actor system terminated.");
            this.actorSystem?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
