//--------------------------------------------------------------------------------------------------
// <copyright file="TickPublisher.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a generic publisher for <see cref="Tick"/> data.
    /// </summary>
    public sealed class TickPublisher : ComponentBase
    {
        private readonly IChannelPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickPublisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="publisher">The tick publisher.</param>
        public TickPublisher(IComponentryContainer container, IChannelPublisher publisher)
        : base(NautilusService.Data, container)
        {
            this.publisher = publisher;

            this.RegisterHandler<Tick>(this.OnMessage);
        }

        private void OnMessage(Tick message)
        {
            this.publisher.Publish(
                message.Symbol.ToString().ToLower(),
                message.ToString());
        }
    }
}
