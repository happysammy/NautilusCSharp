//--------------------------------------------------------------------------------------------------
// <copyright file="TickPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Publishers
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides a publisher for <see cref="Tick"/> data.
    /// </summary>
    public sealed class TickPublisher : Publisher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TickPublisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public TickPublisher(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<Tick>(this.OnMessage);
        }

        private void OnMessage(Tick tick)
        {
            this.Publish(
                Encoding.UTF8.GetBytes(tick.Symbol.Value),
                Encoding.UTF8.GetBytes(tick.ToString()));
        }
    }
}
