// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory to create the systems data bus.
    /// </summary>
    public static class DataBusFactory
    {
        /// <summary>
        /// Creates and returns a new message bus adapter.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <returns>The messaging adapter.</returns>
        public static DataBusAdapter Create(IComponentryContainer container)
        {
            return new DataBusAdapter(
                new DataBus<Tick>(container),
                new DataBus<BarData>(container),
                new DataBus<Instrument>(container));
        }
    }
}
