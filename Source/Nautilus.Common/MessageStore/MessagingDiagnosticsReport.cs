// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingDiagnosticsReport.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using NautechSystems.CSharp.Annotations;

    /// <summary>
    /// Represents a report containing summarized messaging diagnostics of a system run.
    /// </summary>
    [Immutable]
    public sealed class MessagingDiagnosticsReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingDiagnosticsReport"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="totalCount"> The total count.</param>
        /// <param name="isUnopenedCount">The unopened count.</param>
        /// <param name="averageDeliveryTime">The average delivery time.</param>
        /// <param name="fastestDeliveryTime">The fastest delivery time.</param>
        /// <param name="slowestDeliveryTime">The slowest delivery time.</param>
        public MessagingDiagnosticsReport(
            string messageType,
            int totalCount,
            int isUnopenedCount,
            double averageDeliveryTime,
            double fastestDeliveryTime,
            double slowestDeliveryTime)
        {
            this.MessageType = messageType;
            this.FastestDeliveryTime = fastestDeliveryTime;
            this.AverageDeliveryTime = averageDeliveryTime;
            this.SlowestDeliveryTime = slowestDeliveryTime;
            this.TotalCount = totalCount;
            this.IsUnopenedCount = isUnopenedCount;
        }

        /// <summary>
        /// Gets the reports message type.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// Gets the reports message fastest delivery time.
        /// </summary>
        public double FastestDeliveryTime { get; }

        /// <summary>
        /// Gets the reports message average delivery time.
        /// </summary>
        public double AverageDeliveryTime { get; }

        /// <summary>
        /// Gets the reports message slowest delivery time.
        /// </summary>
        public double SlowestDeliveryTime { get; }

        /// <summary>
        /// Gets the reports message total count.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the reports count of unopened messages.
        /// </summary>
        public int IsUnopenedCount { get; }
    }
}
