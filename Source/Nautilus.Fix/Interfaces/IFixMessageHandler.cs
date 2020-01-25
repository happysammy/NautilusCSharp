//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.Interfaces
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using QuickFix.FIX44;

    /// <summary>
    /// The adapter for FIX message handlers.
    /// </summary>
    public interface IFixMessageHandler
    {
        /// <summary>
        /// Initializes the FIX data gateway.
        /// </summary>
        /// <param name="gateway">The data gateway.</param>
        void InitializeGateway(IDataGateway gateway);

        /// <summary>
        /// Initializes the FIX trading gateway.
        /// </summary>
        /// <param name="gateway">The trading gateway.</param>
        void InitializeGateway(IEndpoint gateway);

        /// <summary>
        /// Handles <see cref="BusinessMessageReject"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(BusinessMessageReject message);

        /// <summary>
        /// Handles <see cref="Email"/> message.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(Email message);

        /// <summary>
        /// Handles <see cref="SecurityList"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(SecurityList message);

        /// <summary>
        /// Handles <see cref="QuoteStatusReport"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(QuoteStatusReport message);

        /// <summary>
        /// Handles <see cref="TradingSessionStatus"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(TradingSessionStatus message);

        /// <summary>
        /// Handles <see cref="MarketDataRequestReject"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(MarketDataRequestReject message);

        /// <summary>
        /// Handles <see cref="MarketDataSnapshotFullRefresh"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(MarketDataSnapshotFullRefresh message);

        /// <summary>
        /// Handles <see cref="CollateralInquiryAck"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(CollateralInquiryAck message);

        /// <summary>
        /// Handles <see cref="CollateralReport"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(CollateralReport message);

        /// <summary>
        /// Handles <see cref="RequestForPositionsAck"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(RequestForPositionsAck message);

        /// <summary>
        /// Handles <see cref="PositionReport"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(PositionReport message);

        /// <summary>
        /// Handles <see cref="OrderCancelReject"/> messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(OrderCancelReject message);

        /// <summary>
        /// Handles <see cref="ExecutionReport"/> messages.
        /// </summary>
        /// <param name="message">The report.</param>
        void OnMessage(ExecutionReport message);
    }
}
