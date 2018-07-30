//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdPostfixRemover.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Removes the '_R#' from any modified order identifier.
    /// </summary>
    [Immutable]
    public static class OrderIdPostfixRemover
    {
        /// <summary>
        /// Removes any characters after and including '_R' from the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="string"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static string Remove(string orderId)
        {
            Validate.NotNull(orderId, nameof(orderId));

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
