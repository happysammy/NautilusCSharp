//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
        /// Initializes the brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        void InitializeBrokerageGateway(IBrokerageGateway gateway);

        /// <summary>
        /// The on business message reject.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnBusinessMessageReject(BusinessMessageReject message);

        /// <summary>
        /// The on security list.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnSecurityList(SecurityList message);

        /// <summary>
        /// The on collateral inquiry acknowledgement.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnCollateralInquiryAck(CollateralInquiryAck message);

        /// <summary>
        /// The on collateral report.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnCollateralReport(CollateralReport message);

        /// <summary>
        /// The on request for positions acknowledgement.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnRequestForPositionsAck(RequestForPositionsAck message);

        /// <summary>
        /// The on market data snapshot full refresh.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMarketDataSnapshotFullRefresh(MarketDataSnapshotFullRefresh message);

        /// <summary>
        /// The on order cancel reject.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnOrderCancelReject(OrderCancelReject message);

        /// <summary>
        /// The on execution report.
        /// </summary>
        /// <param name="report">The report.</param>
        void OnExecutionReport(ExecutionReport report);

        /// <summary>
        /// The on position report.
        /// </summary>
        /// <param name="report">The message.</param>
        void OnPositionReport(PositionReport report);
    }
}
