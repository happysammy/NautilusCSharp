//--------------------------------------------------------------------------------------------------
// <copyright file="MessageFinder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    /// <summary>
    /// The message finder.
    /// </summary>
    public static class MessageFinder
    {
        /// <summary>
        /// The find event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="envelopeList">The envelope list.</param>
        /// <returns>The <see cref="SignalEvent"/>.</returns>
        public static T FindSignalEvent<T>(
            IEnumerable<Envelope<EventMessage>> envelopeList) where T : Signal
        {
            return envelopeList.Select(envelope => envelope.Open(StubZonedDateTime.UnixEpoch()).Event)
               .Where(e => e is SignalEvent)
                // ReSharper disable once SuspiciousTypeConversion.Global
                .Cast<T>()
               .FirstOrDefault();
        }

        /// <summary>
        /// The find order event.
        /// </summary>
        /// <param name="envelopeList">The envelope list.</param>
        /// <param name="eventToFind">The event to find.</param>
        /// <returns>The <see cref="OrderEvent"/>.</returns>
        public static OrderEvent FindOrderEvent(IEnumerable<Envelope<EventMessage>> envelopeList, Type eventToFind)
        {
            return envelopeList
               .Select(envelope => envelope.Open(StubZonedDateTime.UnixEpoch()).Event)
               .Where(@event => @event is OrderEvent)
               .Cast<OrderEvent>()
               .FirstOrDefault(orderEvent => orderEvent.GetType() == eventToFind);
        }
    }
}
