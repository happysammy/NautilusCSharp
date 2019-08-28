//--------------------------------------------------------------------------------------------------
// <copyright file="DataRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Requests
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a request for data.
    /// </summary>
    [Immutable]
    public sealed class DataRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRequest"/> class.
        /// </summary>
        /// <param name="query">The request data query.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestTimestamp">The request timestamp.</param>
        public DataRequest(
            Dictionary<string, string> query,
            Guid requestId,
            ZonedDateTime requestTimestamp)
            : base(
                typeof(DataRequest),
                requestId,
                requestTimestamp)
        {
            Debug.NotDefault(requestId, nameof(requestId));
            Debug.NotDefault(requestTimestamp, nameof(requestTimestamp));

            this.Query = query;
        }

        /// <summary>
        /// Gets the requests data query.
        /// </summary>
        public Dictionary<string, string> Query { get; }
    }
}
