// -------------------------------------------------------------------------------------------------
// <copyright file="OrderIdPostfixRemover.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The immutable static <see cref="OrderIdPostfixRemover"/> class. Removes the '_R#' from any
    /// modified order identifier.
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
