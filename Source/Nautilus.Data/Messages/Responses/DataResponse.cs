//--------------------------------------------------------------------------------------------------
// <copyright file="DataResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Responses
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a data response.
    /// </summary>
    [Immutable]
    public sealed class DataResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataResponse"/> class.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="dataEncoding">The response data encoding.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="responseTimestamp">The response timestamp.</param>
        public DataResponse(
            byte[] data,
            DataEncoding dataEncoding,
            Guid correlationId,
            Guid responseId,
            ZonedDateTime responseTimestamp)
            : base(
                typeof(DataResponse),
                correlationId,
                responseId,
                responseTimestamp)
        {
            Debug.NotDefault(responseId, nameof(responseId));
            Debug.NotDefault(responseTimestamp, nameof(responseTimestamp));

            this.Data = data;
            this.DataEncoding = dataEncoding;
        }

        /// <summary>
        /// Gets the responses data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Gets the responses data encoding.
        /// </summary>
        public DataEncoding DataEncoding { get; }
    }
}
