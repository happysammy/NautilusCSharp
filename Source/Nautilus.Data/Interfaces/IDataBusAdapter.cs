//--------------------------------------------------------------------------------------------------
// <copyright file="IDataBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a means for components to subscribe to and unsubscribe from various data types.
    /// </summary>
    public interface IDataBusAdapter
    {
        /// <summary>
        /// Subscribe the given endpoint to data of type T.
        /// </summary>
        /// <typeparam name="T">The data type to subscribe to.</typeparam>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Subscribe<T>(IEndpoint subscriber, Guid id, ZonedDateTime timestamp);

        /// <summary>
        /// Unsubscribe the given endpoint from data of type T.
        /// </summary>
        /// <typeparam name="T">The data type to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Unsubscribe<T>(IEndpoint subscriber, Guid id, ZonedDateTime timestamp);
    }
}
