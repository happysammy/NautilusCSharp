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
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a means of controlling services within the system.
    /// </summary>
    public class SystemController : ComponentBusConnectedBase
    {
        private readonly ActorSystem actorSystem;
        private readonly string actorSystemName;
        private readonly MessagingAdapter messagingAdapter;
        private readonly Switchboard switchboard;
        private readonly ReadOnlyList<NautilusService> services;

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
                LabelFactory.Component(nameof(SystemController)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(actorSystem, nameof(actorSystem));

            this.actorSystem = actorSystem;
            this.actorSystemName = actorSystem.Name;
            this.messagingAdapter = messagingAdapter;
            this.switchboard = switchboard;
            this.services = switchboard.Services;
        }

        /// <summary>
        /// Starts the <see cref="Nautilus"/> system.
        /// </summary>
        public void Start()
        {
            // Allow system to initialize.
            Task.Delay(1000).Wait();

            var initializeSwitchboard =
                new InitializeSwitchboard(
                    this.switchboard,
                    this.NewGuid(),
                    this.TimeNow());

            this.messagingAdapter.Send(initializeSwitchboard);

            // Allow messaging system to initialize.
            Task.Delay(300).Wait();

            var start = new SystemStart(this.NewGuid(), this.TimeNow());
            this.services.ForEach(s => this.Send(s, start));
        }

        /// <summary>
        /// Shuts down the <see cref="Nautilus"/> system.
        /// </summary>
        public void Shutdown()
        {
            var shutdown = new SystemShutdown(this.NewGuid(), this.TimeNow());
            this.services.ForEach(s => this.Send(s, shutdown));

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
