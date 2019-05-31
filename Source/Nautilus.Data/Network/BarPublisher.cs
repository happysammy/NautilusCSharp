//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides a publisher for <see cref="Bar"/> data.
    /// </summary>
    public class BarPublisher : Publisher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public BarPublisher(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<(BarType BarType, Bar Bar)>(this.OnMessage);
        }

        private void OnMessage((BarType BarType, Bar Bar) data)
        {
            this.Publish(
                Encoding.UTF8.GetBytes(data.BarType.ToString()),
                Encoding.UTF8.GetBytes(data.Bar.ToString()));
        }
    }
}
