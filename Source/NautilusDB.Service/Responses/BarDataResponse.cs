//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service.Responses
{
    using Nautilus.Core.Validation;
    using Nautilus.Database.Types;
    using ServiceStack;

    [Api("Market Data Response")]
    public class BarDataResponse
    {
        public BarDataResponse(
            bool isSuccess,
            string message,
            BarDataFrame barData)
        {
            Validate.NotNull(message, nameof(message));
            Validate.NotNull(barData, nameof(barData));

            this.IsSuccess = isSuccess;
            this.Message = message;
            this.BarData = barData;
        }

        /// <summary>
        /// Gets a value indicating whether the response result is successful.
        /// </summary>
        [ApiMember(Name = "Is Success", Description = "The result of the response", IsRequired = true)]
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the responses message.
        /// </summary>
        [ApiMember(Name = "Message", Description = "The response message", IsRequired = true)]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the responses market data (if success).
        /// </summary>
        [ApiMember(Name = "Market Data", Description = "The response market data (if successful)", IsRequired = false)]
        public BarDataFrame BarData { get; private set; }

        /// <summary>
        /// Gets the responses status.
        /// </summary>
        [ApiMember(Name = "Response Status", Description = "The response status from the request", IsRequired = false)]
        public ResponseStatus ResponseStatus { get; private set; }
    }
}
