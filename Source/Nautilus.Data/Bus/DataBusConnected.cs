//--------------------------------------------------------------------------------------------------
// <copyright file="DataBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Bus
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class DataBusConnected : Component
    {
        private readonly IDataBusAdapter dataBusAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        protected DataBusConnected(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter)
            : base(container)
        {
            this.dataBusAdapter = dataBusAdapter;
        }

        /// <summary>
        /// Subscribe to the data type.
        /// </summary>
        /// <typeparam name="T">The data type to subscribe to.</typeparam>
        protected void Subscribe<T>()
        {
            this.dataBusAdapter.Subscribe<T>(this.Mailbox, this.NewGuid(), this.TimeNow());
        }

        /// <summary>
        /// Unsubscribe from the data type.
        /// </summary>
        /// <typeparam name="T">The data type to unsubscribe from.</typeparam>
        protected void Unsubscribe<T>()
        {
            this.dataBusAdapter.Subscribe<T>(this.Mailbox, this.NewGuid(), this.TimeNow());
        }
    }
}
