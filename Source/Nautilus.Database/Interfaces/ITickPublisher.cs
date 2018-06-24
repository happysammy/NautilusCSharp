//--------------------------------------------------------------------------------------------------
// <copyright file="ITickPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Interfaces
{
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The interface for publishing ticks to subscibers.
    /// </summary>
    public interface ITickPublisher
    {
        /// <summary>
        /// Publishes the given tick to subscribers.
        /// </summary>
        /// <param name="tick">The tick to publish.</param>
        void Publish(Tick tick);
    }
}
