//--------------------------------------------------------------------------------------------------
// <copyright file="DataResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Responses
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
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
        /// <param name="dataType">The response data type.</param>
        /// <param name="metaData">The response metadata.</param>
        /// <param name="dataEncoding">The response data encoding.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="responseTimestamp">The response timestamp.</param>
        public DataResponse(
            byte[] data,
            string dataType,
            DataEncoding dataEncoding,
            Dictionary<string, string> metaData,
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
            this.DataType = dataType;
            this.DataEncoding = dataEncoding;
            this.Metadata = metaData;
        }

        /// <summary>
        /// Gets the responses data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Gets the responses data type.
        /// </summary>
        public string DataType { get; }

        /// <summary>
        /// Gets the responses data encoding.
        /// </summary>
        public DataEncoding DataEncoding { get; }

        /// <summary>
        /// Gets the responses metadata.
        /// </summary>
        public Dictionary<string, string> Metadata { get; }
    }
}
