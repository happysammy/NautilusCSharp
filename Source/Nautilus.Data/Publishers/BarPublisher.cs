//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Publishers
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Events;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a generic publisher for <see cref="Bar"/> data.
    /// </summary>
    public sealed class BarPublisher : ComponentBase
    {
        private readonly IChannelPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarPublisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="publisher">The bar publisher.</param>
        public BarPublisher(IComponentryContainer container, IChannelPublisher publisher)
            : base(NautilusService.Data, container)
        {
            this.publisher = publisher;

            this.RegisterHandler<BarClosed>(this.OnMessage);
        }

        private void OnMessage(BarClosed message)
        {
            this.publisher.Publish(
                message.BarType.ToChannelString(),
                message.Bar.ToString());
        }
    }
}
