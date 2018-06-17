//--------------------------------------------------------------------------------------------------
// <copyright file="IBarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Interfaces
{
    using Nautilus.DomainModel.Events;

    /// <summary>
    /// The interface for publishing bar data events to subscribers.
    /// </summary>
    public interface IBarPublisher
    {
        /// <summary>
        /// Publish the given bar data event to all subscribers.
        /// </summary>
        /// <param name="barEvent">The bar data event.</param>
        void Publish(BarDataEvent barEvent);
    }
}
