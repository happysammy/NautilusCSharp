//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.Interfaces
{
    using Nautilus.Common.Interfaces;
    using QuickFix.FIX44;

    /// <summary>
    /// The adapter for FIX message handlers.
    /// </summary>
    public interface IFixMessageHandler
    {
        /// <summary>
        /// Initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        void InitializeGateway(IFixGateway gateway);

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(BusinessMessageReject message);

        /// <summary>
        /// Handles security list messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(SecurityList message);

        /// <summary>
        /// Handles quote status report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(QuoteStatusReport message);

        /// <summary>
        /// Handles collateral inquiry acknowledgement messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(CollateralInquiryAck message);

        /// <summary>
        /// Handles collateral report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(CollateralReport message);

        /// <summary>
        /// Handles request for positions acknowledgement messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(RequestForPositionsAck message);

        /// <summary>
        /// Handles market data request reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(MarketDataRequestReject message);

        /// <summary>
        /// Handles market data snapshot full refresh messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(MarketDataSnapshotFullRefresh message);

        /// <summary>
        /// Handles order cancel reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(OrderCancelReject message);

        /// <summary>
        /// Handles execution report messages.
        /// </summary>
        /// <param name="message">The report.</param>
        void OnMessage(ExecutionReport message);

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(PositionReport message);
    }
}
