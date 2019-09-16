//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdPostfixRemover.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Removes the '_R#' from any modified order identifier.
    /// </summary>
    public static class OrderIdPostfixRemover
    {
        /// <summary>
        /// Returns a modified order identifier from the given order identifier.
        /// Removes any characters after and including '_R'.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The modified order identifier <see cref="string"/>.</returns>
        public static string Remove(string orderId)
        {
            Debug.NotEmptyOrWhiteSpace(orderId, nameof(orderId));

            var orderIdToString = orderId;
            var index = orderIdToString.LastIndexOf("_R", StringComparison.CurrentCulture);

            if (index > 0)
            {
                // Removes the _R# from any modified order id (not ReSharper).
                orderIdToString = orderIdToString.Substring(0, index);
            }

            return orderIdToString;
        }
    }
}
