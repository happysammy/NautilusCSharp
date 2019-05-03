//--------------------------------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common
{
    using System;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel.Factories;
    using NautilusMQ;

    /// <summary>
    /// Provides a means of controlling services within the system.
    /// </summary>
    public class SystemController : ComponentBusConnectedBase
    {
        private readonly ReadOnlyList<Address> components;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="switchboard">The switchboard.</param>
        public SystemController(
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Switchboard switchboard)
            : base(
                NautilusService.Core,
                LabelFactory.Create(nameof(SystemController)),
                container,
                messagingAdapter)
        {
            this.components = switchboard.Addresses;

            var initializeSwitchboard =
                new InitializeSwitchboard(
                    switchboard,
                    this.NewGuid(),
                    this.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);
        }

        /// <summary>
        /// Starts the system.
        /// </summary>
        public override void Start()
        {
            var start = new StartSystem(this.NewGuid(), this.TimeNow());

            foreach (var component in this.components)
            {
                this.Send(component, start);
            }
        }

        /// <summary>
        /// Shuts down the system.
        /// </summary>
        public override void Stop()
        {
            var shutdown = new ShutdownSystem(this.NewGuid(), this.TimeNow());

            foreach (var component in this.components)
            {
                this.Send(component, shutdown);
            }

            Task.Delay(1000).Wait();
            this.ShutdownActorSystem();
        }

        private void ShutdownActorSystem()
        {
            this.Log.Information($"System shutting down...");
            this.Log.Information($"System terminated.");
            GC.SuppressFinalize(this);
        }
    }
}
