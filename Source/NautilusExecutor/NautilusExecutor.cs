//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Contains the Nautilus Executor system.
    /// </summary>
    public sealed class NautilusExecutor : ComponentBusConnectedBase
    {
        private readonly SystemController systemController;
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusExecutor"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="systemController">The system controller.</param>
        /// <param name="fixClient">The FIX client.</param>
        public NautilusExecutor(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            SystemController systemController,
            IFixClient fixClient)
            : base(
                NautilusService.Core,
                LabelFactory.Create(nameof(NautilusExecutor)),
                container,
                messagingAdapter)
        {
            this.systemController = systemController;
            this.fixClient = fixClient;
        }

        /// <summary>
        /// Starts the system.
        /// </summary>
        public override void Start()
        {
            this.fixClient.Connect();

            while (!this.fixClient.IsConnected)
            {
                // Wait for connection.
            }
        }

        /// <summary>
        /// Shuts down the system.
        /// </summary>
        public override void Stop()
        {
            this.fixClient.Disconnect();

            this.systemController.Stop();
        }
    }
}
