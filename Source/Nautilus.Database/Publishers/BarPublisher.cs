//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Publishers
{
    using System.Collections.Generic;
    using Grpc.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Messages.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Providers a generic publisher for <see cref="Bar"/> data.
    /// </summary>
    public class BarPublisher : ActorComponentBase
    {
        private readonly Dictionary<BarType, Channel> subscribers;

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        public BarPublisher(IComponentryContainer container)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(BarPublisher)),
                container)
        {
            Validate.NotNull(container, nameof(container));

            this.Receive<BarClosed>(msg => this.OnMessage(msg));
        }

        private void OnMessage(Subscribe<Symbol> message)
        {

        }

        private void OnMessage(Unsubscribe<Symbol> message)
        {

        }

        private void OnMessage(BarClosed message)
        {
            var barType = message.BarType;

            var client = this.subscribers[barType].ConnectAsync();
        }
    }
}
