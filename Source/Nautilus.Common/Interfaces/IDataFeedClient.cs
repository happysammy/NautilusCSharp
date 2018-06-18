//--------------------------------------------------------------------------------------------------
// <copyright file="IDataFeedClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The adapter for FIX data feed client.
    /// </summary>
    public interface IDataFeedClient
    {
        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        Broker Broker { get; }

        /// <summary>
        /// Returns a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Initializes the FIX session. Performs actions on logon.
        /// </summary>
        void InitializeSession();

        /// <summary>
        /// The request market data subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void RequestMarketDataSubscribe(Symbol symbol);
    }
}
