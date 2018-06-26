//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a gateway and anticorruption layer into the system.
    /// </summary>
    public interface IDataGateway
    {
        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        Broker Broker { get; }

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects the brokerage client.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects the brokerage client.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Requests market data for the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void RequestMarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Requests market data for all symbols from the brokerage.
        /// </summary>
        void RequestMarketDataSubscribeAll();

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage,
        /// and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Updates the given instruments in the instrument repository.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        void OnInstrumentsUpdate(IReadOnlyCollection<Instrument> instruments, string responseId, string result);

        /// <summary>
        /// Event handler for receiving FIX business messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnBusinessMessage(string message);

        /// <summary>
        /// Event handler for receiving FIX Collateral Inquiry Acknowledgements.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        void OnCollateralInquiryAck(string inquiryId, string accountNumber);

        /// <summary>
        /// Event handler for acknowledgement of a request for positions.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="positionRequestId">The position request identifier.</param>
        void OnRequestForPositionsAck(string accountNumber, string positionRequestId);

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        void OnPositionReport(string account);
    }
}
