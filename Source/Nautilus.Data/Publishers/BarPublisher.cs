//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Publishers
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a generic publisher for <see cref="Bar"/> data.
    /// </summary>
    public sealed class BarPublisher : ActorComponentBase
    {
        private readonly IChannelPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarPublisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="publisher">The bar publisher.</param>
        public BarPublisher(IComponentryContainer container, IChannelPublisher publisher)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(BarPublisher)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(publisher, nameof(publisher));

            this.publisher = publisher;

            // Setup message handling.
            this.Receive<BarClosed>(this.OnMessage);
            this.Receive<Subscribe<Symbol>>(this.OnMessage);
            this.Receive<Unsubscribe<Symbol>>(this.OnMessage);
        }

        private void OnMessage(Subscribe<Symbol> message)
        {
            // Not implemented.
        }

        private void OnMessage(Unsubscribe<Symbol> message)
        {
            // Not implemented.
        }

        private void OnMessage(BarClosed message)
        {
            Debug.NotNull(message, nameof(message));

            this.publisher.Publish(
                message.BarType.ToChannel(),
                message.Bar.ToString());
        }
    }
}
