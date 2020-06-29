//--------------------------------------------------------------------------------------------------
// <copyright file="DataRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Requests
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a request for data.
    /// </summary>
    [Immutable]
    public sealed class DataRequest : Request
    {
        private static readonly Type EventType = typeof(DataRequest);

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
                EventType,
                requestId,
                requestTimestamp)
        {
            Condition.NotEmpty(query, nameof(query));
            Debug.NotDefault(requestId, nameof(requestId));
            Debug.NotDefault(requestTimestamp, nameof(requestTimestamp));

            this.Query = query;
        }

        /// <summary>
        /// Gets the requests data query.
        /// </summary>
        public Dictionary<string, string> Query { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="DataRequest"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(DataRequest)}(Query={this.Query.Print()}, Id={this.Id})";
    }
}
