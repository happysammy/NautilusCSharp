//--------------------------------------------------------------------------------------------------
// <copyright file="DataResponse.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Data.Messages.Responses
{
    using System;
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
        private static readonly Type EventType = typeof(DataResponse);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResponse"/> class.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="dataType">The response data type.</param>
        /// <param name="dataEncoding">The response data encoding.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="responseTimestamp">The response timestamp.</param>
        public DataResponse(
            byte[] data,
            string dataType,
            DataEncoding dataEncoding,
            Guid correlationId,
            Guid responseId,
            ZonedDateTime responseTimestamp)
            : base(
                EventType,
                correlationId,
                responseId,
                responseTimestamp)
        {
            Condition.NotEmptyOrWhiteSpace(dataType, nameof(dataType));
            Debug.NotDefault(correlationId, nameof(correlationId));
            Debug.NotDefault(responseId, nameof(responseId));
            Debug.NotDefault(responseTimestamp, nameof(responseTimestamp));

            this.Data = data;
            this.DataType = dataType;
            this.DataEncoding = dataEncoding;
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
    }
}
